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
        public string guardarCita(Cita c)
        {
            int id = obtenerId();
            string mensaje = "Ocurrió un error";
            string query = $"insert into {tabla} (id_Cita, Folio, id_Pac, Motivo_Cita, Fecha_Cita, Hora_Cita, id_Emp, Estatus_Cita) values (@id_Cita, @Folio, @id_Pac, @Motivo_Cita, @Fecha_Cita, @Hora_Cita, @id_Emp, @Estatus_Cita)";
            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.AddWithValue("@id_Cita", id);
            cmd.Parameters.AddWithValue("@Folio", c.Folio);
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
                    mensaje = "Cita guardada correctamente";
                }

            }
            catch(Exception ex)
            {
                return mensaje;
            }
            finally
            {
                con.Close();
            }
            return mensaje;
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



    }
}