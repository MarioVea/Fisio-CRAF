using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Services
{
    public class TratamientoService
    {
        SqlConnection con = new SqlConnection();
        Conexion conexion = new Conexion();

        public TratamientoService()
        {
            con.ConnectionString = conexion.conexion();
        }

        public string guardar(Tratamiento t)
        {
            string resp = "";
            try
            {
                con.Open();
                // Verificamos si existe por ID
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM Salud.Tratamiento WHERE id_Trata = @id_Trata", con);
                checkCmd.Parameters.AddWithValue("@id_Trata", t.id_Trata);
                int count = (int)checkCmd.ExecuteScalar();

                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;

                if (count > 0)
                {
                    // Update
                    cmd.CommandText = "UPDATE Salud.Tratamiento SET Nombre_Trata = @nombre, Descrip_Trata = @descrip WHERE id_Trata = @id_Trata";
                    resp = "Tratamiento actualizado correctamente";
                }
                else
                {
                    // Insert
                    cmd.CommandText = "INSERT INTO Salud.Tratamiento (id_Trata, Nombre_Trata, Descrip_Trata) VALUES (@id_Trata, @nombre, @descrip)";
                    resp = "Tratamiento guardado correctamente";
                }

                cmd.Parameters.AddWithValue("@id_Trata", t.id_Trata);
                cmd.Parameters.AddWithValue("@nombre", t.Nombre_Trata);
                cmd.Parameters.AddWithValue("@descrip", t.Descrip_Trata);

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                resp = "Error: " + ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return resp;
        }

        public string eliminar(int id_Trata)
        {
            string resp = "";
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Salud.Tratamiento WHERE id_Trata = @id_Trata", con);
                cmd.Parameters.AddWithValue("@id_Trata", id_Trata);
                int filasAfectadas = cmd.ExecuteNonQuery();

                if(filasAfectadas > 0)
                    resp = "Tratamiento eliminado correctamente";
                else
                    resp = "No se encontró el tratamiento para eliminar";
            }
            catch (Exception ex)
            {
                resp = "Error: " + ex.Message;
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return resp;
        }

        public List<Tratamiento> cargarDg(string filtro)
        {
            List<Tratamiento> lista = new List<Tratamiento>();
            try
            {
                con.Open();
                string query = "SELECT * FROM Salud.Tratamiento WHERE Nombre_Trata LIKE @filtro OR CAST(id_Trata AS VARCHAR) = @id_filtro";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
                cmd.Parameters.AddWithValue("@id_filtro", filtro);
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    Tratamiento t = new Tratamiento();
                    t.id_Trata = Convert.ToInt32(dr["id_Trata"]);
                    t.Nombre_Trata = dr["Nombre_Trata"].ToString();
                    t.Descrip_Trata = dr["Descrip_Trata"].ToString();
                    lista.Add(t);
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return lista;
        }

        public int consecutivo()
        {
            int id = 1;
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("SELECT ISNULL(MAX(id_Trata), 0) + 1 FROM Salud.Tratamiento", con);
                id = (int)cmd.ExecuteScalar();
            }
            catch (Exception)
            {
            }
            finally
            {
                if (con.State == ConnectionState.Open) con.Close();
            }
            return id;
        }
    }
}