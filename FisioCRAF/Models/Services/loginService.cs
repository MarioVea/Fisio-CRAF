using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Services
{
    public class loginService
    {
        static Conexion conexion = new Conexion();
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();


        public loginService()
        {
            con.ConnectionString = conexion.conexion();
        }

        public string iniciarSesion(string usuario,string password)
        {
            string mensaje = "Ocurrió un error";

            string query = "select * from Persona.Empleados where Contraseña = @password and Nombre_Emp = @usuario";
            cmd.CommandText= query;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@usuario", usuario);
            SqlDataReader reader;

            try
            {
                con.Open();
                reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    mensaje = "Inicio de sesión exitoso";
                }
                return mensaje;

            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error: {ex.Message}";
                return mensaje;
            }
            finally
            {
                con.Close();
            }
        }




    }
}