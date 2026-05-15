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

        public List<Cita> obtenerCitas(int folio)
        {
            var citas = new List<Cita>();
            string query = $"select c.id_Cita,c.Folio,c.id_Pac,concat(p.Nombre_Pac,' ',p.ApellidoP_Pac,' ',p.ApellidoM_Pac) as Nombre,c.Motivo_Cita,c.Fecha_Cita,c.Hora_Cita,c.id_Emp,c.Estatus_Cita from Datos.Cita c inner join Persona.Pacientes p on p.id_Pac = c.id_Pac where c.Folio = @folio";
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@folio", folio);
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
    }
}