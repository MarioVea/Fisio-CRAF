using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FisioCRAF.Models.Services
{
    public class EmpleadoService
    {
        string tabla = "Persona.Empleados";
        static Conexion conexion = new Conexion();
        SqlConnection con = new SqlConnection();

        public EmpleadoService()
        {
            con.ConnectionString = conexion.conexion();
        }


        public string guardarEmpleado(Empleado e)
        {
            int id = obtenerId();
            string mensaje = "Ocurrió un error";
            string query = $"insert into {tabla} (id_Emp, Clave_Emp, Nombre_Emp, ApellidoP_Emp, ApellidoM_Emp, Tipo_Emp, Contraseña, Telefono_Emp, CedulaProfesional, id_Esp, Estatus_Emp) VALUES (@id_Emp, @Clave_Emp, @Nombre_Emp, @ApellidoP_Emp, @ApellidoM_Emp, @Tipo_Emp, @Contraseña, @Telefono_Emp, @CedulaProfesional, @id_Esp, @Estatus_Emp)";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id_Emp", id);
            cmd.Parameters.AddWithValue("@Clave_Emp", e.Clave_Emp);
            cmd.Parameters.AddWithValue("@Nombre_Emp", e.Nombre_Emp);
            cmd.Parameters.AddWithValue("@ApellidoP_Emp", e.ApellidoP_Emp);
            cmd.Parameters.AddWithValue("@ApellidoM_Emp", e.ApellidoM_Emp);
            cmd.Parameters.AddWithValue("@Tipo_Emp", e.tipo_Emp);
            cmd.Parameters.AddWithValue("@Contraseña", e.Contraseña);
            cmd.Parameters.AddWithValue("@Telefono_Emp", e.Telefono_Emp);
            cmd.Parameters.AddWithValue("@CedulaProfesional", e.CedulaProfesional);
            cmd.Parameters.AddWithValue("@id_Esp", e.id_Esp);
            cmd.Parameters.AddWithValue("@Estatus_Emp", e.estatus_Emp);

            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery() > 0)
                mensaje = "Empleado guardado correctamente";
            }
            catch(Exception ex)
            {
                mensaje = $"Ocurrió un error: {ex.Message}";
            }
            finally
            {
                con.Close();
            }
            return mensaje;
        }

        public string actualizarEmpleado(Empleado e)
        {
            string mensaje = "Ocurrió un error";
            string query = $"update {tabla} set Clave_Emp = @Clave_Emp, Nombre_Emp = @Nombre_Emp, ApellidoP_Emp = @ApellidoP_Emp, ApellidoM_Emp = @ApellidoM_Emp, Tipo_Emp = @Tipo_Emp, Contraseña = @Contraseña, Telefono_Emp = @Telefono_Emp, CedulaProfesional = @CedulaProfesional, id_Esp = @id_Esp, Estatus_Emp = @Estatus_Emp where id_Emp = @id_Emp";
            SqlCommand cmd = new SqlCommand(query, con);
            cmd.Parameters.AddWithValue("@id_Emp", e.id_Emp);
            cmd.Parameters.AddWithValue("@Clave_Emp", e.Clave_Emp);
            cmd.Parameters.AddWithValue("@Nombre_Emp", e.Nombre_Emp);
            cmd.Parameters.AddWithValue("@ApellidoP_Emp", e.ApellidoP_Emp);
            cmd.Parameters.AddWithValue("@ApellidoM_Emp", e.ApellidoM_Emp);
            cmd.Parameters.AddWithValue("@Tipo_Emp", e.tipo_Emp);
            cmd.Parameters.AddWithValue("@Contraseña", e.Contraseña);
            cmd.Parameters.AddWithValue("@Telefono_Emp", e.Telefono_Emp);
            cmd.Parameters.AddWithValue("@CedulaProfesional", e.CedulaProfesional);
            cmd.Parameters.AddWithValue("@id_Esp", e.id_Esp);
            cmd.Parameters.AddWithValue("@Estatus_Emp", e.estatus_Emp);

            try
            {
                con.Open();
                if (cmd.ExecuteNonQuery() > 0)
                {
                    mensaje = "Empleado actualizado correctamente";
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error: {ex.Message}";
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
            string query = $"select isnull(max(id_Emp),0) + 1 as id from {tabla}";
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


        public List<Especialidad> obtenerEspecialidades()
        {
            List<Especialidad> especialidades = new List<Especialidad>();
            string mensaje = "Ocurrió un error";
            string query = $"select id_Esp,Nombre_Esp from Persona.Especialidad";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataReader reader;
            con.Open();
            try
            {
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    especialidades.Add(new Especialidad
                    {
                        id_Esp = int.Parse(reader["id_Esp"].ToString()),
                        Nombre_Esp = reader["Nombre_Esp"].ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                especialidades = null;
            }
            finally
            {
                con.Close();
            }

            return especialidades;
        }


        public List<Empleado> obtenerEmpleados(string nombre,int busqueda)
        {
            List<Empleado> empleados = new List<Empleado>();
            string query = "";
            if(busqueda == 0)
                query = $"SELECT  top 5 E.id_Emp, E.Clave_Emp, E.Nombre_Emp, E.ApellidoP_Emp,\r\n\t\tE.ApellidoM_Emp,E.Tipo_Emp,\r\n\t\t\r\n\t\t\tCASE E.Tipo_Emp\r\n\t\t\t\t\tWHEN 0 THEN 'Fisioterapeuta'\r\n\t\t\t\t\tWHEN 1 THEN 'Prestador de Servicios'\r\n\t\t\tEND AS Tipo,\r\n\r\n\t\t\tE.Contraseña,\r\n\t\t\tE.Telefono_Emp,\r\n\t\t\tE.CedulaProfesional,\r\n\t\t\tpe.id_Esp,\r\n\t\t\tpe.Nombre_Esp,\r\n\t\t\tE.Estatus_Emp,\r\n\r\n\t\t\tCASE E.Estatus_Emp\r\n\t\t\t\t\tWHEN 0 THEN 'Inactivo'\r\n\t\t\t\t\tWHEN 1 THEN 'Activo'\r\n\t\t\tEND AS Estatus\r\n\r\n\t\tFROM Persona.Empleados AS E\r\n\t\tINNER JOIN Persona.Especialidad AS pe ON E.id_Esp = pe.id_Esp\r\n\t\twhere concat (E.Nombre_Emp, ' ',E.ApellidoP_Emp,' ',E.ApellidoM_Emp) like '%{nombre}%'";
            else
                query = $"SELECT E.id_Emp, E.Clave_Emp, E.Nombre_Emp, E.ApellidoP_Emp,\r\n\t\tE.ApellidoM_Emp,E.Tipo_Emp,\r\n\t\t\r\n\t\t\tCASE E.Tipo_Emp\r\n\t\t\t\t\tWHEN 0 THEN 'Fisioterapeuta'\r\n\t\t\t\t\tWHEN 1 THEN 'Prestador de Servicios'\r\n\t\t\tEND AS Tipo,\r\n\r\n\t\t\tE.Contraseña,\r\n\t\t\tE.Telefono_Emp,\r\n\t\t\tE.CedulaProfesional,\r\n\t\t\tpe.id_Esp,\r\n\t\t\tpe.Nombre_Esp,\r\n\t\t\tE.Estatus_Emp,\r\n\r\n\t\t\tCASE E.Estatus_Emp\r\n\t\t\t\t\tWHEN 0 THEN 'Inactivo'\r\n\t\t\t\t\tWHEN 1 THEN 'Activo'\r\n\t\t\tEND AS Estatus\r\n\r\n\t\tFROM Persona.Empleados AS E\r\n\t\tINNER JOIN Persona.Especialidad AS pe ON E.id_Esp = pe.id_Esp\r\n\t\twhere concat (E.Nombre_Emp, ' ',E.ApellidoP_Emp,' ',E.ApellidoM_Emp) like '%{nombre}%'";
            SqlCommand cmd = new SqlCommand(query, con);
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            con.Open();
            da.Fill(dt);
            foreach (DataRow row in dt.Rows)
            {
                Empleado empleado = new Empleado();
                empleado.id_Emp = Convert.ToInt32(row["id_Emp"]);
                empleado.Clave_Emp = row["Clave_Emp"].ToString();
                empleado.Nombre_Emp = row["Nombre_Emp"].ToString();
                empleado.ApellidoP_Emp = row["ApellidoP_Emp"].ToString();
                empleado.ApellidoM_Emp = row["ApellidoM_Emp"].ToString();
                empleado.tipo_Emp = Convert.ToInt32(row["Tipo_Emp"]);
                empleado.Tipo_Emp = row["Tipo"].ToString();
                empleado.Contraseña = row["Contraseña"].ToString();
                empleado.Telefono_Emp = row["Telefono_Emp"].ToString();
                empleado.CedulaProfesional = row["CedulaProfesional"].ToString();
                empleado.id_Esp = Convert.ToInt32(row["id_Esp"]);
                empleado.nombre_Esp = row["Nombre_Esp"].ToString();
                empleado.estatus_Emp = Convert.ToInt32(row["Estatus_Emp"]);
                empleado.Estatus_Emp = row["Estatus"].ToString();



                if (Convert.ToBoolean(row["Estatus_Emp"]) == false)
                    empleado.estatus_Emp = 0;
                else
                    empleado.estatus_Emp = 1;
                empleados.Add(empleado);
            }
            con.Close();
            return empleados;
        }





    }
}