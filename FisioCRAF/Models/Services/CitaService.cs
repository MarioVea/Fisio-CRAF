using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Services
{
    public class CitaService
    {
        string tabla = "Datos.Cita";
        static Conexion conexion = new Conexion();
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();


        public CitaService()
        {
            con.ConnectionString = conexion.conexion();
        }
        public bool ValidarCita(Cita c, out string mensaje)
        {
            mensaje = "";
            bool esValida = true;
            string query = $"select Hora_Cita, id_Emp, id_Pac from {tabla} where Fecha_Cita = @FechaCitaVal and Estatus_Cita != 2 and id_Cita != @idCitaVal";
            SqlCommand cmdVal = new SqlCommand(query, con);
            cmdVal.Parameters.AddWithValue("@FechaCitaVal", c.Fecha_Cita);
            cmdVal.Parameters.AddWithValue("@idCitaVal", c.id_Cita);
            
            try
            {
                con.Open();
                using (SqlDataReader reader = cmdVal.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TimeSpan horaExistente = TimeSpan.Parse(reader["Hora_Cita"].ToString());
                        int empExistente = int.Parse(reader["id_Emp"].ToString());
                        int pacExistente = int.Parse(reader["id_Pac"].ToString());

                        double diferenciaMinutos = Math.Abs((c.Hora_Cita - horaExistente).TotalMinutes);
                        
                        if (diferenciaMinutos < 15)
                        {
                            if (empExistente == c.id_Emp)
                            {
                                esValida = false;
                                mensaje = "El fisioterapeuta ya tiene una cita programada en ese horario (debe haber al menos 15 minutos de diferencia).";
                                break;
                            }
                            if (pacExistente == c.id_Pac)
                            {
                                esValida = false;
                                mensaje = "El paciente ya tiene una cita programada en ese horario (debe haber al menos 15 minutos de diferencia).";
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                esValida = false;
                mensaje = "Ocurrió un error al validar el horario de la cita.";
            }
            finally
            {
                con.Close();
            }
            
            return esValida;
        }

        public bool guardarCita(Cita c)
        {
            int id = obtenerId();
            bool respuesta = false;
            string query = $"insert into {tabla} (id_Cita, Folio, id_Pac, Motivo_Cita, Fecha_Cita, Hora_Cita, id_Emp, Estatus_Cita) values (@id_Cita, @Folio, @id_Pac, @Motivo_Cita, @Fecha_Cita, @Hora_Cita, @id_Emp, @Estatus_Cita)";
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@id_Cita", id);
            cmd.Parameters.AddWithValue("@Folio", id);
            cmd.Parameters.AddWithValue("@id_Pac", c.id_Pac);
            cmd.Parameters.AddWithValue("@Motivo_Cita", c.Motivo_Cita);
            cmd.Parameters.AddWithValue("@Fecha_Cita", c.Fecha_Cita);
            cmd.Parameters.AddWithValue("@Hora_Cita", c.Hora_Cita);
            cmd.Parameters.AddWithValue("@id_Emp", c.id_Emp);
            cmd.Parameters.AddWithValue("@Estatus_Cita", c.Estatus_Cita);
            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    respuesta = true;
                }

            }
            catch (Exception ex)
            {
                return respuesta;
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        public bool actualizarCita(Cita c)
        {
            bool respuesta = false;
            string query = $"update {tabla} set id_Pac = @id_Pac, Motivo_Cita = @Motivo_Cita, Fecha_Cita = @Fecha_Cita, Hora_Cita = @Hora_Cita, id_Emp = @id_Emp, Estatus_Cita = @Estatus_Cita where id_Cita = @id_Cita";
            SqlCommand cmdUpdate = new SqlCommand(query, con);
            cmdUpdate.Parameters.AddWithValue("@id_Pac", c.id_Pac);
            cmdUpdate.Parameters.AddWithValue("@Motivo_Cita", c.Motivo_Cita);
            cmdUpdate.Parameters.AddWithValue("@Fecha_Cita", c.Fecha_Cita);
            cmdUpdate.Parameters.AddWithValue("@Hora_Cita", c.Hora_Cita);
            cmdUpdate.Parameters.AddWithValue("@id_Emp", c.id_Emp);
            cmdUpdate.Parameters.AddWithValue("@Estatus_Cita", c.Estatus_Cita);
            cmdUpdate.Parameters.AddWithValue("@id_Cita", c.id_Cita);
            try
            {
                con.Open();
                if (cmdUpdate.ExecuteNonQuery() > 0)
                {
                    respuesta = true;
                }
            }
            catch (Exception ex)
            {
                return respuesta;
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        public bool eliminarCita(int id_Cita)
        {
            bool respuesta = false;
            string query = $"delete from {tabla} where id_Cita = @id_Cita";
            SqlCommand cmdDelete = new SqlCommand(query, con);
            cmdDelete.Parameters.AddWithValue("@id_Cita", id_Cita);
            try
            {
                con.Open();
                if (cmdDelete.ExecuteNonQuery() > 0)
                {
                    respuesta = true;
                }
            }
            catch (Exception ex)
            {
                return respuesta;
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        public int obtenerId()
        {
            int id = 0;
            string query = $"select isnull(max(id_Cita),0) + 1 as id from {tabla}";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader;
            con.Open();
            reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                id = int.Parse(reader["id"].ToString());
            }
            con.Close();
            return id;
        }

        public List<Empleado> obtenerFisios()
        {
            var fisios = new List<Empleado>();

            string query = $"select id_Emp, Nombre_Emp, ApellidoP_Emp, ApellidoM_Emp from Persona.Empleados where Tipo_Emp = 1 and Estatus_Emp = 1";
            cmd.CommandText = query;
            cmd.Connection = con;
            SqlDataReader reader;

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    fisios.Add(new Empleado
                    {
                        id_Emp = int.Parse(reader["id_Emp"].ToString()),
                        Nombre_Emp = reader["Nombre_Emp"].ToString(),
                        ApellidoP_Emp = reader["ApellidoP_Emp"].ToString(),
                        ApellidoM_Emp = reader["ApellidoM_Emp"].ToString()
                    });
                }
                return fisios;
            }
            catch (Exception ex)
            {
                return fisios;
            }
            finally
            {
                con.Close();
            }

        }

        public List<Cita> obtenerCitas(string nombre)
        {
            var citas = new List<Cita>();
            string query = "select c.id_Cita,c.Folio,c.id_Pac,concat(p.Nombre_Pac,' ',p.ApellidoP_Pac,' ',p.ApellidoM_Pac) as Nombre,c.Motivo_Cita,c.Fecha_Cita,c.Hora_Cita,c.id_Emp,c.Estatus_Cita from Datos.Cita c inner join Persona.Pacientes p on p.id_Pac = c.id_Pac where concat(p.Nombre_Pac,' ',p.ApellidoP_Pac,' ',p.ApellidoM_Pac) like '%' + @nombre + '%'";
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@nombre", nombre);
            SqlDataReader reader;
            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    citas.Add(new Cita
                    {
                        id_Cita = int.Parse(reader["id_Cita"].ToString()),
                        Folio = int.Parse(reader["Folio"].ToString()),
                        id_Pac = int.Parse(reader["id_Pac"].ToString()),
                        Motivo_Cita = reader["Motivo_Cita"].ToString(),
                        Fecha_Cita = DateTime.Parse(reader["Fecha_Cita"].ToString()),
                        Hora_Cita = TimeSpan.Parse(reader["Hora_Cita"].ToString()),
                        id_Emp = int.Parse(reader["id_Emp"].ToString()),
                        Estatus_Cita = int.Parse(reader["Estatus_Cita"].ToString()),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
                return citas;
            }
            catch (Exception ex)
            {
                return citas;
            }
            finally
            {
                con.Close();
            }
        }


        public List<string> obtenerHorasOcupadas(int idEmp, DateTime fecha)
        {
            var horas = new List<string>();
            string query = $"select Hora_Cita from {tabla} where id_Emp = @idEmp and Fecha_Cita = @fecha and Estatus_Cita != 2";
            SqlCommand cmdHoras = new SqlCommand(query, con);
            cmdHoras.Parameters.AddWithValue("@idEmp", idEmp);
            cmdHoras.Parameters.AddWithValue("@fecha", fecha);
            try
            {
                con.Open();
                using (SqlDataReader reader = cmdHoras.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TimeSpan hora = TimeSpan.Parse(reader["Hora_Cita"].ToString());
                        horas.Add(hora.Hours.ToString("D2") + ":00");
                    }
                }
            }
            catch (Exception ex)
            {
                return horas;
            }
            finally
            {
                con.Close();
            }
            return horas;
        }

        public List<Cita> obtenerCitasPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            var citas = new List<Cita>();
            string query = "select c.id_Cita,c.Folio,c.id_Pac,concat(p.Nombre_Pac,' ',p.ApellidoP_Pac,' ',p.ApellidoM_Pac) as Nombre,c.Motivo_Cita,c.Fecha_Cita,c.Hora_Cita,c.id_Emp,c.Estatus_Cita from Datos.Cita c inner join Persona.Pacientes p on p.id_Pac = c.id_Pac where c.Fecha_Cita >= @fechaInicio and c.Fecha_Cita <= @fechaFin";
            SqlCommand cmdFecha = new SqlCommand(query, con);
            cmdFecha.Parameters.AddWithValue("@fechaInicio", fechaInicio);
            cmdFecha.Parameters.AddWithValue("@fechaFin", fechaFin);
            SqlDataReader reader;
            try
            {
                con.Open();
                reader = cmdFecha.ExecuteReader();
                while (reader.Read())
                {
                    citas.Add(new Cita
                    {
                        id_Cita = int.Parse(reader["id_Cita"].ToString()),
                        Folio = int.Parse(reader["Folio"].ToString()),
                        id_Pac = int.Parse(reader["id_Pac"].ToString()),
                        Motivo_Cita = reader["Motivo_Cita"].ToString(),
                        Fecha_Cita = DateTime.Parse(reader["Fecha_Cita"].ToString()),
                        Hora_Cita = TimeSpan.Parse(reader["Hora_Cita"].ToString()),
                        id_Emp = int.Parse(reader["id_Emp"].ToString()),
                        Estatus_Cita = int.Parse(reader["Estatus_Cita"].ToString()),
                        Nombre = reader["Nombre"].ToString()
                    });
                }
                return citas;
            }
            catch (Exception ex)
            {
                return citas;
            }
            finally
            {
                con.Close();
            }
        }
    }
}