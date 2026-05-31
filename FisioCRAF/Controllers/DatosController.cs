using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FisioCRAF.Controllers
{
    public class DatosController : Controller
    {

        CitaService cs = new CitaService();
        // GET: Datos
        public async Task<ActionResult> Cita()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> obtenerFisios()
        {
            try
            {
                var fisios = cs.obtenerFisios();
                return Json(fisios, JsonRequestBehavior.AllowGet);

            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { error = "Ocurrió un error al obtener los fisioterapeutas." }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public async Task<ActionResult> guardarCita(Cita cita)
        {
            try
            {
                string mensaje = "";
                if (!cs.ValidarCita(cita, out mensaje))
                {
                    return Json(new { respuesta = false, mensaje = mensaje });
                }

                var respuesta = cs.guardarCita(cita);
                return Json(new {respuesta = respuesta});
            }
            catch(Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new {respuesta = ex.Message});
            }
        }

        [HttpGet]
        public async Task<ActionResult> obtenerCitas(string nombre)
        {
            try
            {
                var respuesta = cs.obtenerCitas(nombre);
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new {respuesta = "Ocurrió un error al obtener las citas"});
            }
        }

        [HttpGet]
        public async Task<ActionResult> obtenerCitasPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var respuesta = cs.obtenerCitasPorFecha(fechaInicio, fechaFin);
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new { respuesta = "Ocurrió un error al obtener las citas por fecha" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public async Task<ActionResult> actualizarCita(Cita cita)
        {
            try
            {
                string mensaje = "";
                if (!cs.ValidarCita(cita, out mensaje))
                {
                    return Json(new { respuesta = false, mensaje = mensaje });
                }

                var respuesta = cs.actualizarCita(cita);
                return Json(new { respuesta = respuesta });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { respuesta = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult> eliminarCita(int id_Cita)
        {
            try
            {
                var respuesta = cs.eliminarCita(id_Cita);
                return Json(new { respuesta = respuesta });
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { respuesta = ex.Message });
            }
        }



    }
}