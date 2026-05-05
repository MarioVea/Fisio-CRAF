using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Services
{
    public class PacienteService
    {
        string tabla = "Persona.Pacientes";
        static Conexion conexion = new Conexion();
        SqlConnection con = new SqlConnection();

        public PacienteService()
        {
            con.ConnectionString = (conexion.conexion());
        }
        public string guardarPaciente(Paciente paciente)
        {
            int id = obtenerId();
            string mensaje = "Ocurrió un error";
            string query = $"insert into {tabla} (id_Pac,Nombre_Pac,ApellidoP_Pac,ApellidoM_Pac,Telefono_Pac,Fecha_Nacimiento,Genero) values (@id_Pac,@Nombre_Pac,@ApellidoP_Pac,@ApellidoM_Pac,@Telefono_Pac,@Fecha_Nacimiento,@Genero)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id_Pac", id);
            cmd.Parameters.AddWithValue("@Nombre_Pac", paciente.Nombre_Pac);
            cmd.Parameters.AddWithValue("@ApellidoP_Pac", paciente.ApellidoP_Pac);
            cmd.Parameters.AddWithValue("@ApellidoM_Pac", paciente.ApellidoM_Pac);
            cmd.Parameters.AddWithValue("@Telefono_Pac", paciente.Telefono_Pac);
            cmd.Parameters.AddWithValue("@Fecha_Nacimiento", paciente.Fecha_Nacimiento);
            cmd.Parameters.AddWithValue("@Genero", paciente.Genero);

            con.Open();
            if (cmd.ExecuteNonQuery() > 0)
            {
                mensaje = "Paciente guardado correctamente";
            }
            con.Close();
            return mensaje;
        }
        public int obtenerId()
        {
            int id = 0;
            string query = $"select isnull(max(id_Pac),0) + 1 as id from {tabla}";
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

        public List<Paciente> obtenerPacientes(string nombre)
        {
            List<Paciente> pacientes = new List<Paciente>();
            string query = $"select top 3  * from Persona.Pacientes where Nombre_Pac like '%{nombre}%'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                Paciente paciente = new Paciente();
                paciente.id_Pac = Convert.ToInt32(row["id_Pac"]);
                paciente.Nombre_Pac = row["Nombre_Pac"].ToString();
                paciente.ApellidoP_Pac = row["ApellidoP_Pac"].ToString();
                paciente.ApellidoM_Pac = row["ApellidoM_Pac"].ToString();
                paciente.Telefono_Pac = row["Telefono_Pac"].ToString();
                paciente.Fecha_Nacimiento = Convert.ToDateTime(row["Fecha_Nacimiento"]);
                paciente.Genero = row["Genero"].ToString();
                pacientes.Add(paciente);
            }
            con.Close();
            return pacientes;
        }
        public List<Paciente> obtenerPacientesSinLimite(string nombre)
        {
            List<Paciente> pacientes = new List<Paciente>();
            string query = $"select  * from Persona.Pacientes where concat(Nombre_Pac, ' ',ApellidoP_Pac,' ', ApellidoM_Pac ) like '%{nombre}%'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                Paciente paciente = new Paciente();
                paciente.id_Pac = Convert.ToInt32(row["id_Pac"]);
                paciente.Nombre_Pac = row["Nombre_Pac"].ToString();
                paciente.ApellidoP_Pac = row["ApellidoP_Pac"].ToString();
                paciente.ApellidoM_Pac = row["ApellidoM_Pac"].ToString();
                paciente.Telefono_Pac = row["Telefono_Pac"].ToString();
                paciente.Fecha_Nacimiento = Convert.ToDateTime(row["Fecha_Nacimiento"]);
                paciente.Genero = row["Genero"].ToString();
                pacientes.Add(paciente);
            }
            con.Close();
            return pacientes;
        }


        public string ActualizarPaciente(Paciente paciente)
        {
            string mensaje = "Ocurrió un error";
            string query = $"update Persona.Pacientes set Nombre_Pac = @Nombre_Pac, ApellidoP_Pac = @ApellidoP_Pac, ApellidoM_Pac = @ApellidoM_Pac, Telefono_Pac = @Telefono_Pac, Fecha_Nacimiento = @Fecha_Nacimiento, Genero = @Genero where id_Pac = @id_Pac";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id_Pac", paciente.id_Pac);
            cmd.Parameters.AddWithValue("@Nombre_Pac", paciente.Nombre_Pac);
            cmd.Parameters.AddWithValue("@ApellidoP_Pac", paciente.ApellidoP_Pac);
            cmd.Parameters.AddWithValue("@ApellidoM_Pac", paciente.ApellidoM_Pac);
            cmd.Parameters.AddWithValue("@Telefono_Pac", paciente.Telefono_Pac);
            cmd.Parameters.AddWithValue("@Fecha_Nacimiento", paciente.Fecha_Nacimiento);
            cmd.Parameters.AddWithValue("@Genero", paciente.Genero);

            con.Open();
            if (cmd.ExecuteNonQuery() > 0)
            {
                mensaje = "Paciente actualizado correctamente";
            }
            con.Close();
            return mensaje;


        }
        public string EliminarPaciente(int id)
        {
            string mensaje = "Ocurrió un error";
            string query = $"delete from {tabla} where id_Pac = @id_Pac";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id_Pac", id);

            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    mensaje = "Paciente eliminado correctamente";
                }
                return mensaje;

            }
            catch(Exception ex)
            {
             return $"{mensaje} : {ex.Message}";   
            }
            finally
            {
                con.Close(); 
            }


        }



    }
}