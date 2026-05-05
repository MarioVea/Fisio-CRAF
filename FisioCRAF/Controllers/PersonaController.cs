using FisioCRAF.Models;
using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace FisioCRAF.Controllers
{
    public class PersonaController : Controller
    {
        PacienteService ps = new PacienteService();
        EmpleadoService es = new EmpleadoService();
        // GET: Persona
        public async Task<ActionResult> Paciente()
        {
            if (TempData["mensaje"] == null)
            {
                TempData["mensaje"] = string.Empty;
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> actualizarEmpleado()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var body = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    var request = serializer.Deserialize<EmpleadoRequest>(body);
                    var empleado = new Empleado
                    {
                        id_Emp = request.id_Emp,
                        Clave_Emp = request.Clave_Emp,
                        Nombre_Emp = request.Nombre_Emp,
                        ApellidoP_Emp = request.ApellidoP_Emp,
                        ApellidoM_Emp = request.ApellidoM_Emp,
                        Contraseña = request.Contraseña,
                        Telefono_Emp = request.Telefono_Emp,
                        CedulaProfesional = request.CedulaProfesional,
                        tipo_Emp = request.tipo_Emp,
                        estatus_Emp = request.estatus_Emp,
                        id_Esp = request.id_Esp
                    };
                    string respuesta = es.actualizarEmpleado(empleado);
                    return Json(new { message = respuesta });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<ActionResult> eliminarPaciente()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var body = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    var request = serializer.Deserialize<EliminarPacienteRequest>(body);
                    var respuesta = ps.EliminarPaciente(request.id_Pac);
                    return Json(new { message = respuesta });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = $"Ocurrió un error: {ex.Message}" });
            }
        }




        public async Task<ActionResult> Empleado()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> guardarPaciente(Paciente paciente)
        {
            try
            {
                var respuesta = ps.guardarPaciente(paciente);
                return Json(new { message = respuesta });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<ActionResult> buscarPacientes(string nombre)
        {
            var pacientes = ps.obtenerPacientesSinLimite(nombre);

            return Json(pacientes, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> actualizarPaciente(Paciente paciente)
        {
            var respuesta = ps.ActualizarPaciente(paciente);
            return Json(new { message = respuesta });

        }

        [HttpGet]
        public async Task<ActionResult> obtenerEspecialidades()
        {
            var especialidades = es.obtenerEspecialidades();
            return Json(especialidades, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async Task<ActionResult> guardarEmpleado()
        {
            try
            {
                using (var reader = new StreamReader(Request.InputStream))
                {
                    var body = reader.ReadToEnd();
                    var serializer = new JavaScriptSerializer();
                    var request = serializer.Deserialize<EmpleadoRequest>(body);
                    var empleado = new Empleado
                    {
                        Clave_Emp = request.Clave_Emp,
                        Nombre_Emp = request.Nombre_Emp,
                        ApellidoP_Emp = request.ApellidoP_Emp,
                        ApellidoM_Emp = request.ApellidoM_Emp,
                        Contraseña = request.Contraseña,
                        Telefono_Emp = request.Telefono_Emp,
                        CedulaProfesional = request.CedulaProfesional,
                        tipo_Emp = request.tipo_Emp,
                        estatus_Emp = request.estatus_Emp,
                        id_Esp = request.id_Esp
                    };
                    string respuesta = es.guardarEmpleado(empleado);
                    return Json(new { message = respuesta });
                }
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        private class EmpleadoRequest
        {
            public int id_Emp { get; set; }
            public string Clave_Emp { get; set; }
            public string Nombre_Emp { get; set; }
            public string ApellidoP_Emp { get; set; }
            public string ApellidoM_Emp { get; set; }
            public int tipo_Emp { get; set; }
            public string Contraseña { get; set; }
            public string Telefono_Emp { get; set; }
            public string CedulaProfesional { get; set; }
            public int id_Esp { get; set; }
            public int estatus_Emp { get; set; }
        }

        private class EliminarPacienteRequest
        {
            public int id_Pac { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> obtenerEmpleados(string nombre, int busqueda)
        {
            var empleados = es.obtenerEmpleados(nombre,busqueda);
            return Json(empleados,JsonRequestBehavior.AllowGet);
        }
    }
}