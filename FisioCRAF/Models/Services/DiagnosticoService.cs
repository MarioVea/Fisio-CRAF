using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FisioCRAF.Models.Services
{
    public class DiagnosticoService
    {
        /// <summary>
        /// Busca diagnósticos por paciente o motivo de cita
        /// </summary>
        public List<object> BuscarDiagnosticos(string busqueda)
        {
            var lista = new List<object>();
            try
            {
                using (var con = new System.Data.SqlClient.SqlConnection(new Conexion().conexion()))
                {
                    con.Open();
                    string sql = @"
                        SELECT 
                            c.id_Cita, c.Motivo_Cita, c.Fecha_Cita, c.id_Pac, c.id_Emp,
                            d.id_Diag, d.Num_Diag, d.id_Consul,
                            p.Nombre_Pac + ' ' + p.ApellidoP_Pac + ' ' + p.ApellidoM_Pac AS Paciente,
                            e.Nombre_Emp + ' ' + e.ApellidoP_Emp + ' ' + e.ApellidoM_Emp AS Fisioterapeuta
                        FROM Datos.Cita c
                        LEFT JOIN Diagnostico.Diagnostico d ON c.id_Cita = d.id_Cita
                        JOIN Persona.Pacientes p ON c.id_Pac = p.id_Pac
                        JOIN Persona.Empleados e ON c.id_Emp = e.id_Emp
                        WHERE 1=1 ";

                    if (!string.IsNullOrEmpty(busqueda))
                    {
                        sql += @" AND (c.Motivo_Cita LIKE @busqueda 
                                    OR CAST(ISNULL(d.Num_Diag, '') AS VARCHAR) LIKE @busqueda 
                                    OR (p.Nombre_Pac + ' ' + p.ApellidoP_Pac + ' ' + p.ApellidoM_Pac) LIKE @busqueda)";
                    }

                    var cmd = new System.Data.SqlClient.SqlCommand(sql, con);
                    if (!string.IsNullOrEmpty(busqueda))
                    {
                        cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");
                    }

                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        lista.Add(new
                        {
                            id_Diag = reader["id_Diag"] != DBNull.Value ? (int)reader["id_Diag"] : 0,
                            Num_Diag = reader["Num_Diag"] != DBNull.Value ? (int)reader["Num_Diag"] : 0,
                            id_Cita = (int)reader["id_Cita"],
                            id_Consul = reader["id_Consul"] != DBNull.Value ? (int)reader["id_Consul"] : (int?)null,
                            Motivo_Cita = reader["Motivo_Cita"].ToString(),
                            Fecha_Cita = Convert.ToDateTime(reader["Fecha_Cita"]).ToString("yyyy-MM-dd"),
                            Paciente = reader["Paciente"].ToString(),
                            Fisioterapeuta = reader["Fisioterapeuta"].ToString(),
                            id_Pac = (int)reader["id_Pac"],
                            id_Emp = (int)reader["id_Emp"]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error BuscarDiagnosticos: " + ex.Message);
            }
            return lista;
        }

        /// <summary>
        /// Obtiene detalle completo de un diagnóstico por id
        /// </summary>
        public object ObtenerDiagnosticoPorId(int idDiag)
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    var diag = db.Diagnosticos.Find(idDiag);
                    if (diag == null) return null;

                    // Detalle diagnóstico
                    var detallesDiag = db.DetallesDiag.Where(d => d.id_Diag == idDiag).ToList();

                    // Detalle ejercicios
                    var detallesEjer = (from de in db.DetallesEjer
                                        join ej in db.Ejercicios on de.id_Ejercicio equals ej.id_Ejercicio
                                        join cat in db.CategoriasEjercicio on ej.id_CatEjer equals cat.id_CatEjer
                                        where de.id_Diag == idDiag
                                        select new
                                        {
                                            de.id_DetalleEjer,
                                            de.id_Ejercicio,
                                            ej.Nombre_Ejer,
                                            cat.Nombre_CatEjer,
                                            de.Series,
                                            de.Repeticiones
                                        }).ToList();

                    // Detalle cuerpo humano con tratamientos
                    var detallesCH = (from ch in db.DetallesCH
                                      where ch.id_Diag == idDiag
                                      select new
                                      {
                                          ch.id_DetalleCH,
                                          ch.Nombre_Musculo,
                                          ch.Imag_Musculo,
                                          ch.Descripcion_DiagCH,
                                          ch.id_DetalleTrata
                                      }).ToList();

                    // Para cada detalleCH obtener el tratamiento
                    var detallesCHConTrata = detallesCH.Select(ch =>
                    {
                        string nombreTrata = "";
                        int? idTrata = null;
                        if (ch.id_DetalleTrata.HasValue)
                        {
                            var dt = db.DetallesTrata.Find(ch.id_DetalleTrata.Value);
                            if (dt != null && dt.id_Trata.HasValue)
                            {
                                idTrata = dt.id_Trata;
                                var trata = db.Tratamientos.Find(dt.id_Trata.Value);
                                if (trata != null) nombreTrata = trata.Nombre_Trata;
                            }
                        }
                        return new
                        {
                            ch.id_DetalleCH,
                            ch.Nombre_Musculo,
                            ch.Imag_Musculo,
                            ch.Descripcion_DiagCH,
                            ch.id_DetalleTrata,
                            id_Trata = idTrata,
                            Nombre_Trata = nombreTrata
                        };
                    }).ToList();

                    // Escala dolor para los detalles
                    var detallesDiagConEscala = detallesDiag.Select(dd =>
                    {
                        string escalaDolor = "";
                        if (dd.id_EscalaDolor.HasValue)
                        {
                            var escala = db.EscalasDolor.Find(dd.id_EscalaDolor.Value);
                            if (escala != null) escalaDolor = escala.Valor_Escala;
                        }
                        string nombreLesion = "";
                        if (dd.id_Lesion.HasValue)
                        {
                            var lesion = db.Lesiones.Find(dd.id_Lesion.Value);
                            if (lesion != null) nombreLesion = lesion.Nom_les;
                        }
                        return new
                        {
                            dd.id_DetalleDiag,
                            dd.Tipo_Diag,
                            dd.Nombre_Diag,
                            dd.id_Lesion,
                            Nombre_Lesion = nombreLesion,
                            dd.RadioGrafia,
                            dd.Descrip_Diag,
                            dd.id_EscalaDolor,
                            Valor_Escala = escalaDolor
                        };
                    }).ToList();

                    return new
                    {
                        diag.id_Diag,
                        diag.Num_Diag,
                        diag.id_Cita,
                        diag.id_Consul,
                        DetallesDiag = detallesDiagConEscala,
                        DetallesEjer = detallesEjer,
                        DetallesCH = detallesCHConTrata
                    };
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error ObtenerDiagnosticoPorId: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Guarda un diagnóstico nuevo con todos sus detalles
        /// </summary>
        public bool GuardarDiagnostico(
            int idCita, int? idConsul, 
            List<DetalleDiag> detallesDiag,
            List<DetalleEjerDiag> detallesEjer,
            List<DetalleCH> detallesCH,
            List<int?> tratamientoIds)
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Calcular id y Num_Diag
                            int maxId = db.Diagnosticos.Any() ? db.Diagnosticos.Max(d => d.id_Diag) : 0;
                            int maxNum = db.Diagnosticos.Any() ? db.Diagnosticos.Max(d => d.Num_Diag) : 0;

                            var diagnostico = new DiagnosticoEntity
                            {
                                id_Diag = maxId + 1,
                                Num_Diag = maxNum + 1,
                                id_Cita = idCita,
                                id_Consul = idConsul
                            };
                            db.Diagnosticos.Add(diagnostico);
                            db.SaveChanges();

                            // Guardar DetalleDiag
                            int maxDetalleDiag = db.DetallesDiag.Any() ? db.DetallesDiag.Max(d => d.id_DetalleDiag) : 0;
                            foreach (var dd in detallesDiag)
                            {
                                maxDetalleDiag++;
                                dd.id_DetalleDiag = maxDetalleDiag;
                                dd.id_Diag = diagnostico.id_Diag;
                                db.DetallesDiag.Add(dd);
                            }
                            db.SaveChanges();

                            // Guardar DetalleEjer
                            int maxDetalleEjer = db.DetallesEjer.Any() ? db.DetallesEjer.Max(d => d.id_DetalleEjer) : 0;
                            foreach (var de in detallesEjer)
                            {
                                maxDetalleEjer++;
                                de.id_DetalleEjer = maxDetalleEjer;
                                de.id_Diag = diagnostico.id_Diag;
                                db.DetallesEjer.Add(de);
                            }
                            db.SaveChanges();

                            // Guardar DetalleTrata y DetalleCH
                            int maxDetalleTrata = db.DetallesTrata.Any() ? db.DetallesTrata.Max(d => d.id_DetalleTrata) : 0;
                            int maxDetalleCH = db.DetallesCH.Any() ? db.DetallesCH.Max(d => d.id_DetalleCH) : 0;

                            for (int i = 0; i < detallesCH.Count; i++)
                            {
                                // Crear DetalleTrata
                                int? idDetalleTrata = null;
                                if (tratamientoIds != null && i < tratamientoIds.Count && tratamientoIds[i].HasValue)
                                {
                                    maxDetalleTrata++;
                                    var dt = new DetalleTrata
                                    {
                                        id_DetalleTrata = maxDetalleTrata,
                                        id_Trata = tratamientoIds[i]
                                    };
                                    db.DetallesTrata.Add(dt);
                                    db.SaveChanges();
                                    idDetalleTrata = dt.id_DetalleTrata;
                                }

                                maxDetalleCH++;
                                detallesCH[i].id_DetalleCH = maxDetalleCH;
                                detallesCH[i].id_Diag = diagnostico.id_Diag;
                                detallesCH[i].id_DetalleTrata = idDetalleTrata;
                                db.DetallesCH.Add(detallesCH[i]);
                            }
                            db.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                if (ex.InnerException != null) msg += " | " + ex.InnerException.Message;
                if (ex.InnerException?.InnerException != null) msg += " | " + ex.InnerException.InnerException.Message;
                throw new Exception("Error GuardarDiagnostico: " + msg);
            }
        }

        /// <summary>
        /// Actualiza un diagnóstico existente y sus detalles
        /// </summary>
        public bool ActualizarDiagnostico(
            int idDiag, int? idConsul,
            List<DetalleDiag> detallesDiag,
            List<DetalleEjerDiag> detallesEjer,
            List<DetalleCH> detallesCH,
            List<int?> tratamientoIds)
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            var diagnostico = db.Diagnosticos.Find(idDiag);
                            if (diagnostico == null) return false;

                            diagnostico.id_Consul = idConsul;
                            db.SaveChanges();

                            // Eliminar detalles existentes
                            var oldDetallesDiag = db.DetallesDiag.Where(d => d.id_Diag == idDiag).ToList();
                            db.DetallesDiag.RemoveRange(oldDetallesDiag);

                            var oldDetallesEjer = db.DetallesEjer.Where(d => d.id_Diag == idDiag).ToList();
                            db.DetallesEjer.RemoveRange(oldDetallesEjer);

                            // Obtener ids de DetalleTrata antes de borrar DetalleCH
                            var oldDetallesCH = db.DetallesCH.Where(d => d.id_Diag == idDiag).ToList();
                            var oldDetalleTrataIds = oldDetallesCH
                                .Where(ch => ch.id_DetalleTrata.HasValue)
                                .Select(ch => ch.id_DetalleTrata.Value)
                                .ToList();
                            db.DetallesCH.RemoveRange(oldDetallesCH);

                            // Eliminar DetalleTrata asociados
                            foreach (var dtId in oldDetalleTrataIds)
                            {
                                var dt = db.DetallesTrata.Find(dtId);
                                if (dt != null) db.DetallesTrata.Remove(dt);
                            }
                            db.SaveChanges();

                            // Re-insertar DetalleDiag
                            int maxDetalleDiag = db.DetallesDiag.Any() ? db.DetallesDiag.Max(d => d.id_DetalleDiag) : 0;
                            foreach (var dd in detallesDiag)
                            {
                                maxDetalleDiag++;
                                dd.id_DetalleDiag = maxDetalleDiag;
                                dd.id_Diag = idDiag;
                                db.DetallesDiag.Add(dd);
                            }
                            db.SaveChanges();

                            // Re-insertar DetalleEjer
                            int maxDetalleEjer = db.DetallesEjer.Any() ? db.DetallesEjer.Max(d => d.id_DetalleEjer) : 0;
                            foreach (var de in detallesEjer)
                            {
                                maxDetalleEjer++;
                                de.id_DetalleEjer = maxDetalleEjer;
                                de.id_Diag = idDiag;
                                db.DetallesEjer.Add(de);
                            }
                            db.SaveChanges();

                            // Re-insertar DetalleTrata y DetalleCH
                            int maxDetalleTrata = db.DetallesTrata.Any() ? db.DetallesTrata.Max(d => d.id_DetalleTrata) : 0;
                            int maxDetalleCH = db.DetallesCH.Any() ? db.DetallesCH.Max(d => d.id_DetalleCH) : 0;

                            for (int i = 0; i < detallesCH.Count; i++)
                            {
                                int? idDetalleTrata = null;
                                if (tratamientoIds != null && i < tratamientoIds.Count && tratamientoIds[i].HasValue)
                                {
                                    maxDetalleTrata++;
                                    var dt = new DetalleTrata
                                    {
                                        id_DetalleTrata = maxDetalleTrata,
                                        id_Trata = tratamientoIds[i]
                                    };
                                    db.DetallesTrata.Add(dt);
                                    db.SaveChanges();
                                    idDetalleTrata = dt.id_DetalleTrata;
                                }

                                maxDetalleCH++;
                                detallesCH[i].id_DetalleCH = maxDetalleCH;
                                detallesCH[i].id_Diag = idDiag;
                                detallesCH[i].id_DetalleTrata = idDetalleTrata;
                                db.DetallesCH.Add(detallesCH[i]);
                            }
                            db.SaveChanges();

                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error ActualizarDiagnostico: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Elimina un diagnóstico y todos sus detalles
        /// </summary>
        public bool EliminarDiagnostico(int idDiag)
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    using (var transaction = db.Database.BeginTransaction())
                    {
                        try
                        {
                            // Eliminar DetalleEjer
                            var detallesEjer = db.DetallesEjer.Where(d => d.id_Diag == idDiag).ToList();
                            db.DetallesEjer.RemoveRange(detallesEjer);

                            // Eliminar DetalleCH y sus DetalleTrata
                            var detallesCH = db.DetallesCH.Where(d => d.id_Diag == idDiag).ToList();
                            var detalleTrataIds = detallesCH
                                .Where(ch => ch.id_DetalleTrata.HasValue)
                                .Select(ch => ch.id_DetalleTrata.Value)
                                .ToList();
                            db.DetallesCH.RemoveRange(detallesCH);

                            foreach (var dtId in detalleTrataIds)
                            {
                                var dt = db.DetallesTrata.Find(dtId);
                                if (dt != null) db.DetallesTrata.Remove(dt);
                            }

                            // Eliminar DetalleDiag
                            var detallesDiag = db.DetallesDiag.Where(d => d.id_Diag == idDiag).ToList();
                            db.DetallesDiag.RemoveRange(detallesDiag);

                            // Eliminar Diagnostico
                            var diagnostico = db.Diagnosticos.Find(idDiag);
                            if (diagnostico != null)
                            {
                                db.Diagnosticos.Remove(diagnostico);
                            }

                            db.SaveChanges();
                            transaction.Commit();
                            return true;
                        }
                        catch
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error EliminarDiagnostico: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Obtiene la lista de consultorios
        /// </summary>
        public List<Consultorio> ObtenerConsultorios()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    return db.Consultorios.ToList();
                }
            }
            catch { return new List<Consultorio>(); }
        }

        /// <summary>
        /// Obtiene la lista de escalas de dolor
        /// </summary>
        public List<EscalaDolor> ObtenerEscalasDolor()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    return db.EscalasDolor.ToList();
                }
            }
            catch { return new List<EscalaDolor>(); }
        }

        /// <summary>
        /// Obtiene la lista de tratamientos
        /// </summary>
        public List<Tratamiento> ObtenerTratamientos()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    return db.Tratamientos.ToList();
                }
            }
            catch { return new List<Tratamiento>(); }
        }

        /// <summary>
        /// Obtiene las lesiones con su tipo
        /// </summary>
        public List<object> ObtenerLesiones()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    var lista = (from l in db.Lesiones
                                 join t in db.TipoLesiones on l.id_TipoLes equals t.id_TipoLes
                                 select new
                                 {
                                     l.id_Lesion,
                                     l.id_TipoLes,
                                     t.Nom_TipLes,
                                     l.Nom_les,
                                     l.Grado,
                                     l.Descrip_Les
                                 }).ToList();
                    return lista.Cast<object>().ToList();
                }
            }
            catch { return new List<object>(); }
        }

        /// <summary>
        /// Obtiene tipos de lesiones
        /// </summary>
        public List<TipoLesiones> ObtenerTipoLesiones()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    return db.TipoLesiones.ToList();
                }
            }
            catch { return new List<TipoLesiones>(); }
        }

        /// <summary>
        /// Obtiene ejercicios con su categoría
        /// </summary>
        public List<object> ObtenerEjercicios()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    var lista = (from e in db.Ejercicios
                                 join c in db.CategoriasEjercicio on e.id_CatEjer equals c.id_CatEjer
                                 select new
                                 {
                                     e.id_Ejercicio,
                                     e.Nombre_Ejer,
                                     e.Descrip_Ejer,
                                     e.id_CatEjer,
                                     c.Nombre_CatEjer
                                 }).ToList();
                    return lista.Cast<object>().ToList();
                }
            }
            catch { return new List<object>(); }
        }

        /// <summary>
        /// Obtiene categorías de ejercicios
        /// </summary>
        public List<CategoriaEjercicio> ObtenerCategorias()
        {
            try
            {
                using (var db = new FisioCRAFTContext())
                {
                    return db.CategoriasEjercicio.ToList();
                }
            }
            catch { return new List<CategoriaEjercicio>(); }
        }
    }
}
