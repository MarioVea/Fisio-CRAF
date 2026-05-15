using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace FisioCRAF.Models.Services
{
    public class EjercicioService
    {
       
        string tabla = "Ejercicio.Ejercicios";
        static Conexion conexion = new Conexion();
        SqlConnection con = new SqlConnection();
        SqlCommand cmd = new SqlCommand();

        public EjercicioService()
        {
            con.ConnectionString = conexion.conexion();
        }

        public bool guardarEjercicio(Ejercicio e)
        {
            bool respuesta = false;

            string query = $"insert into {tabla} (id_Ejercicio, Nombre_Ejer, Descrip_Ejer, id_CatEjer, Imag_Ejer) values ((select isnull(max(id_Ejercicio),0)+1 from {tabla}), @Nombre, @Desc, @idCat, @Imagen)";

            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre_Ejer);
            cmd.Parameters.AddWithValue("@Desc", e.Descrip_Ejer);
            cmd.Parameters.AddWithValue("@idCat", e.id_CatEjer);

           
            if (string.IsNullOrEmpty(e.Imag_Ejer))
                cmd.Parameters.AddWithValue("@Imagen", "");
            else
                cmd.Parameters.AddWithValue("@Imagen", e.Imag_Ejer);

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
                throw;
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }
        public List<Ejercicio> busquedaEjercicios(string busqueda)
        {
            var lista = new List<Ejercicio>();

            //filtara los resultados para encontrar coicidencias 
            string consulta = $"select id_Ejercicio, Nombre_Ejer, Descrip_Ejer, Imag_Ejer, id_CatEjer from {tabla} where Nombre_Ejer like @busqueda";

            cmd.CommandText = consulta;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@busqueda", $"%{busqueda}%");

            try
            {
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Ejercicio
                    {
                        id_Ejercicio = int.Parse(reader["id_Ejercicio"].ToString()),
                        Nombre_Ejer = reader["Nombre_Ejer"].ToString(),
                        Descrip_Ejer = reader["Descrip_Ejer"].ToString(),

                        //verifica si la imagen biene nula de la base de datos
                        Imag_Ejer = reader["Imag_Ejer"] == DBNull.Value ? null : reader["Imag_Ejer"].ToString(),
                        id_CatEjer = int.Parse(reader["id_CatEjer"].ToString())
                    });
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                con.Close();
            }
            return lista;
        }

        public bool actualizarEjercicio(Ejercicio e)
        {
            bool respuesta = false;

            string query = $"update {tabla} set Nombre_Ejer = @Nombre, Descrip_Ejer = @Desc, id_CatEjer = @idCat, Imag_Ejer = @Imagen where id_Ejercicio = @id";

            cmd.CommandText = query;
            cmd.Connection = con;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@id", e.id_Ejercicio);
            cmd.Parameters.AddWithValue("@Nombre", e.Nombre_Ejer);
            cmd.Parameters.AddWithValue("@Desc", e.Descrip_Ejer);
            cmd.Parameters.AddWithValue("@idCat", e.id_CatEjer);

            if (string.IsNullOrEmpty(e.Imag_Ejer))
                cmd.Parameters.AddWithValue("@Imagen", "");
            else
                cmd.Parameters.AddWithValue("@Imagen", e.Imag_Ejer);

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
                throw;
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        public bool eliminarEjercicio(int id)
        {
            bool respuesta = false;

            try
            {
                con.Open();

                // Primero eliminar los registros relacionados en DetalleEjerLes
                string queryDetalle = "delete from Lesion.DetalleEjerLes where id_Ejercicio = @id";
                cmd.CommandText = queryDetalle;
                cmd.Connection = con;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();

                // Luego eliminar el ejercicio
                string queryEjercicio = $"delete from {tabla} where id_Ejercicio = @id2";
                cmd.CommandText = queryEjercicio;
                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@id2", id);
                if (cmd.ExecuteNonQuery() > 0)
                {
                    respuesta = true;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                con.Close();
            }
            return respuesta;
        }

        // obtener las categorías
        public List<CategoriaEjercicio> ObtenerCategorias()
        {
            var listaCategorias = new List<CategoriaEjercicio>();
            
            
            string consulta = "select id_CatEjer, Nombre_CatEjer from Ejercicio.CatEjercicio";
            
            SqlCommand cmdCategorias = new SqlCommand(consulta, con);

            try
            {
                con.Open();
                SqlDataReader reader = cmdCategorias.ExecuteReader();
                while (reader.Read())
                {
                    listaCategorias.Add(new CategoriaEjercicio
                    {
                        id_CatEjer = int.Parse(reader["id_CatEjer"].ToString()),
                        Nombre_CatEjer = reader["Nombre_CatEjer"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                con.Close();
            }

            return listaCategorias;
        }
    }
}