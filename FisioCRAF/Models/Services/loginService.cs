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

        public bool iniciarSesion(string usuario,string password)
        {
            bool inicio = false;

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
                    inicio = true;
                }
                return inicio;

            }
            catch (Exception ex)
            {
                return inicio;
            }
            finally
            {
                con.Close();
            }
        }




    }
}