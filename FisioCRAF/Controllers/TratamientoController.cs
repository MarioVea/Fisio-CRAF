using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FisioCRAF.Controllers
{
    public class TratamientoController : Controller
    {
        TratamientoService ts = new TratamientoService();

        
        public ActionResult Index()
        {
            ViewBag.NuevoId = ts.consecutivo();
            return View();
        }

        [HttpPost]
        public ActionResult Guardar(Tratamiento t)
        {
            if (string.IsNullOrWhiteSpace(t.Nombre_Trata))
            {
                return Json(new { success = false, message = "El campo Nombre de Tratamiento no puede ir vacío" });
            }

            if (t.Nombre_Trata.Length > 50)
            {
                return Json(new { success = false, message = "El Nombre del Tratamiento no puede exceder los 50 caracteres" });
            }

            if (string.IsNullOrWhiteSpace(t.Descrip_Trata))
            {
                return Json(new { success = false, message = "El campo Descripción de Tratamiento no puede ir vacío" });
            }

            if (t.Descrip_Trata.Length > 300)
            {
                return Json(new { success = false, message = "La Descripción del Tratamiento no puede exceder los 300 caracteres" });
            }

            string result = ts.guardar(t);
            bool success = result.Contains("correctamente");
            return Json(new { success = success, message = result, nuevoId = ts.consecutivo() });
        }

        [HttpPost]
        public ActionResult Eliminar(int id)
        {
            string result = ts.eliminar(id);
            bool success = result.Contains("correctamente");
            return Json(new { success = success, message = result, nuevoId = ts.consecutivo() });
        }

        [HttpGet]
        public ActionResult CargarDg(string filtro)
        {
            filtro = string.IsNullOrEmpty(filtro) ? "" : filtro;
            var list = ts.cargarDg(filtro);
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerNuevoId()
        {
            return Json(new { nuevoId = ts.consecutivo() }, JsonRequestBehavior.AllowGet);
        }
    }
}