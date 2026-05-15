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
        public async Task<ActionResult> obtenerCitas(int Folio)
        {
            try
            {
                var respuesta = cs.obtenerCitas(Folio);
                return Json(respuesta, JsonRequestBehavior.AllowGet);
            }
            catch
            {
                return Json(new {respuesta = "Ocurrió un error al obtener las citas"});
            }
        }



    }
}