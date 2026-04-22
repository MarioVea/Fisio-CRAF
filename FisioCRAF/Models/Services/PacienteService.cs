using System;
using System.Collections.Generic;
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
        public Paciente obtenerPaciente(int id_Paciente)
        {
            Paciente paciente = new Paciente();
            string query = $"select * from {tabla} where id_Pac = @id";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id",id_Paciente);

            con.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                paciente.id_Pac = Convert.ToInt32(reader["id_Pac"]);
                paciente.Nombre_Pac = reader["Nombre_Pac"].ToString();
                paciente.ApellidoP_Pac = reader["ApellidoP_Pac"].ToString();
                paciente.ApellidoM_Pac = reader["ApellidoM_Pac"].ToString();
                paciente.Telefono_Pac = reader["Telefono_Pac"].ToString();
                paciente.Fecha_Nacimiento = Convert.ToDateTime(reader["Fecha_Nacimiento"]);
                paciente.Genero = reader["Genero"].ToString();
            }
            con.Close();
            return paciente;
        }
    }
}