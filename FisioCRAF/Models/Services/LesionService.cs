using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace FisioCRAF.Models.Services
{
    public class LesionService
    {
        static Conexion conexion = new Conexion();
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();

        public LesionService()
        {
            con.ConnectionString = conexion.conexion();
        }

        //obtener el tipo de lesión
        public List<TipoLesiones> OnbtenerTiposLesion()
        {
            var lista = new List<TipoLesiones>();
            string consulta = "select id_TipoLes, Nom_TipoLes from Lesion.TipoLesiones";

            SqlCommand cmdTipos = new SqlCommand(consulta, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmdTipos.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new TipoLesiones
                    {
                        id_TipoLes = int.Parse(reader["id_TipoLes"].ToString()),
                        Nom_TipLes = reader["Nom_TipoLes"].ToString()
                    });
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error OnbtenerTiposLesion: " + ex.Message); }

            finally { con.Close(); }
            return lista;
        }

        //obtener la categoria de ejersisios
        public List<CategoriaEjercicio> ObtenerCategorias()
        {
            var lista = new List<CategoriaEjercicio>();
            string consulta = "select id_CatEjer, Nombre_CatEjer from Ejercicio.CatEjercicio";
            SqlCommand cmdCat = new SqlCommand(consulta, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmdCat.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new CategoriaEjercicio
                    {
                        id_CatEjer = int.Parse(reader["id_CatEjer"].ToString()),
                        Nombre_CatEjer = reader["Nombre_CatEjer"].ToString()
                    });
                }

            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error OnbtenerTiposLesion: " + ex.Message); }

            finally { con.Close(); }
            return lista;
        }

        //Filtarar los ejercicios por categoría
        public List<Ejercicio> ObtenerEjercicioCategoria(int idCat)
        {
            var lista = new List<Ejercicio>();
            string consulta = "select id_Ejercicio, Nombre_Ejer from Ejercicio.Ejercicios where id_CatEjer = @idCat";
            cmd.CommandText = consulta;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@idCat", idCat);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Ejercicio
                    {
                        id_Ejercicio = int.Parse(reader["id_Ejercicio"].ToString()),
                        Nombre_Ejer = reader["Nombre_Ejer"].ToString()
                    });
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error OnbtenerTiposLesion: " + ex.Message); }

            finally { con.Close(); }
            return lista;
        }

        //obtener los ejercicios ya asignados a una lesión
        public List<Ejercicio> ObtenerEjercicioLesion(int idLesion)
        {
            var lista = new List<Ejercicio>();
            string consulta = @"select e.id_Ejercicio, e.Nombre_Ejer from Lesion.DetalleEjerLes d inner join Ejercicio.Ejercicios e on d.id_Ejercicio = e.id_Ejercicio where d.id_Lesion = @id";
            cmd.CommandText = consulta;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", idLesion);
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Ejercicio
                    {
                        id_Ejercicio = int.Parse(reader["id_Ejercicio"].ToString()),
                        Nombre_Ejer = reader["Nombre_Ejer"].ToString()
                    });
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error OnbtenerTiposLesion: " + ex.Message); }

            finally { con.Close(); }
            return lista;
        }

        //buscar lesiones por el nombre o clave (para el modal)
        public List<Lesion> BusaquedaLesiones(string busqueda)
        {
            var lista = new List<Lesion>();
            string consulta = @"select l.id_Lesion, l.id_TipoLes, t.Nom_TipoLes, l.Nom_Les, l.Grado, l.Descrip_Les from Lesion.Lesiones l inner join Lesion.TipoLesiones t on l.id_TipoLes = t.id_TipoLes where l.Nom_Les like @busqueda OR cast(l.id_Lesion as varchar) like @busqueda";
            cmd.CommandText = consulta;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@busqueda", "%" + busqueda + "%");
            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Lesion
                    {
                        id_Lesion = int.Parse(reader["id_Lesion"].ToString()),
                        id_TipoLes = int.Parse(reader["id_TipoLes"].ToString()),
                        Nombre_TipoLes = reader["Nom_TipoLes"].ToString(),
                        Nom_les = reader["Nom_Les"].ToString(),
                        Grado = reader["Grado"].ToString(),
                        Descrip_Les = reader["Descrip_Les"].ToString()
                    });
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error OnbtenerTiposLesion: " + ex.Message); }

            finally { con.Close(); }
            return lista;
        }

        //obtener todas las ldesiones, para la tabal inferior 
        public List<Lesion> ObtenerTodaLesion()
        {
            var lista = new List<Lesion>();
            string consulta = @"select l.id_Lesion, l.id_TipoLes, t.Nom_TipoLes, l.Nom_Les, l.Grado, l.Descrip_Les from Lesion.Lesiones l inner join Lesion.TipoLesiones t on l.id_TipoLes = t.id_TipoLes";
            SqlCommand cmdALL = new SqlCommand(consulta, con);
            try
            {
                con.Open();
                SqlDataReader reader = cmdALL.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Lesion
                    {
                        id_Lesion = int.Parse(reader["id_Lesion"].ToString()),
                        id_TipoLes = int.Parse(reader["id_TipoLes"].ToString()),
                        Nombre_TipoLes = reader["Nom_TipoLes"].ToString(),
                        Nom_les = reader["Nom_Les"].ToString(),
                        Grado = reader["Grado"].ToString(),
                        Descrip_Les = reader["Descrip_Les"].ToString()
                    });
                }
            }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine("Error OnbtenerTiposLesion: " + ex.Message); }

            finally { con.Close(); }
            return lista;
        }

        // guardar las lesiones con sus ejercicios 
        public bool guardarLesion(Lesion l, int[] ejerciciosIds)
        {
            bool respuesta = false;
            try
            {
                con.Open();
                //inserta la lesión 
                string consulta = @"insert into Lesion.Lesiones (id_Lesion, id_TipoLes, Nom_Les, Grado, Descrip_Les) values ((select isnull(max(id_Lesion),0)+1 from Lesion.Lesiones), @TipoLes, @Nombre, @Grado, @Descrip)";
                cmd.CommandText = consulta;
                cmd.Connection = con;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@TipoLes", l.id_TipoLes);
                cmd.Parameters.AddWithValue("@Nombre", l.Nom_les);
                cmd.Parameters.AddWithValue("@Grado", l.Grado);
                cmd.Parameters.AddWithValue("@Descrip", l.Descrip_Les);
                cmd.ExecuteNonQuery();

                //recuperar los id recien insertados 
                cmd.CommandText = "select max(id_Lesion) from Lesion.Lesiones";
                cmd.Parameters.Clear();
                int nuevoId = (int)cmd.ExecuteScalar();

                //insertar en cada ejercicio en el detalle 
                if (ejerciciosIds != null && ejerciciosIds.Length > 0)
                {
                    foreach (int idEjer in ejerciciosIds)
                    {
                        string consultaDetalle = @"insert into Lesion.DetalleEjerLes (id_DetalleEjerLes, id_Lesion, id_Ejercicio) values ((select isnull(max(id_DetalleEjerLes),0)+1 from Lesion.DetalleEjerLes), @idLesion, @idEjer)";
                        cmd.CommandText = consultaDetalle;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@idLesion", nuevoId);
                        cmd.Parameters.AddWithValue("@idEjer", idEjer);
                        cmd.ExecuteNonQuery();
                    }
                }
                respuesta = true;

            }
            catch (Exception e) { throw; }
            finally { con.Close(); }
            return respuesta;
        }

        //actualizar lesiones 
        public bool ActualizarLesion(Lesion l, int[] ejerciciosIds)
        {
            bool respuesta = false;
            try
            {
                con.Open();
                //actualizar la lesión
                string consulta = @"update Lesion.Lesiones set id_TipoLes = @TipoLes, Nom_Les = @Nombre, Grado = @Grado, Descrip_Les = @Descrip where id_Lesion = @idLesion";
                cmd.CommandText = consulta;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idLesion", l.id_Lesion);
                cmd.Parameters.AddWithValue("@TipoLes", l.id_TipoLes);
                cmd.Parameters.AddWithValue("@Nombre", l.Nom_les);
                cmd.Parameters.AddWithValue("@Grado", l.Grado);
                cmd.Parameters.AddWithValue("@Descrip", l.Descrip_Les);
                cmd.ExecuteNonQuery();

                //borrar los ejercicios antes del detalle
                cmd.CommandText = "delete from Lesion.DetalleEjerLes where id_Lesion = @idLesion";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idLesion", l.id_Lesion);
                cmd.ExecuteNonQuery();

                //insertar los ejercicios nuevos 
                if (ejerciciosIds != null && ejerciciosIds.Length > 0)
                {
                    foreach (int idEjer in ejerciciosIds)
                    {
                        string consultaDetalle = @"insert into Lesion.DetalleEjerLes (id_DetalleEjerLes, id_Lesion, id_Ejercicio) values ((select isnull(max(id_DetalleEjerLes),0)+1 from Lesion.DetalleEjerLes), @idLesion, @idEjer)";
                        cmd.CommandText = consultaDetalle;
                        cmd.Parameters.Clear();
                        cmd.Parameters.AddWithValue("@idLesion", l.id_Lesion);
                        cmd.Parameters.AddWithValue("@idEjer", idEjer);
                        cmd.ExecuteNonQuery();
                    }
                }
                respuesta = true;
            }
            catch (Exception e) { throw; }
            finally { con.Close(); }
            return respuesta;
        }

        //eliminar lesiones priemro el detalle y despues la lesión
        public bool EliminarLesion(int id)
        {
            bool respuesta = false;
            try
            {
                con.Open();

                cmd.CommandText = "delete from Lesion.DetalleEjerLes where id_Lesion = @idLesion";
                cmd.Connection = con;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idLesion", id);
                cmd.ExecuteNonQuery();

                cmd.CommandText = "delete from Lesion.Lesiones where id_Lesion = @idLesion";
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@idLesion", id);

                if (cmd.ExecuteNonQuery() > 0) respuesta = true;
            }
            catch (Exception ex) { throw; }
            finally { con.Close(); }
            return respuesta;
        }
    }
}