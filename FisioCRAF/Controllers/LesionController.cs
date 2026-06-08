using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;

namespace FisioCRAF.Controllers
{
    public class LesionController : Controller
    {
        // GET: lesion
        LesionService ls = new LesionService();

        public ActionResult lesion()
        {
            ViewBag.TiposLesion = ls.OnbtenerTiposLesion();
            ViewBag.Categorias = ls.ObtenerCategorias();

            //grados fijos
            ViewBag.Grado = new List<Grado>
            {
                new Grado { id_Grado = "Leve",     Nombre_Grado = "Leve"     },
                new Grado { id_Grado = "Moderado", Nombre_Grado = "Moderado" },
                new Grado { id_Grado = "Grave",    Nombre_Grado = "Grave"    }
            };
            return View();
        }

        [HttpPost]
        public ActionResult GuardarLesion(string Nombre_Lesion, string Descrip_Lesion, int id_TipoLesion, string id_Grado, int[] ejerciciosIds)
        {
            try
            {
                Lesion nueva = new Lesion
                {
                    id_TipoLes = id_TipoLesion,
                    Nom_les = Nombre_Lesion,
                    Grado = id_Grado,
                    Descrip_Les = Descrip_Lesion
                };

                bool guardado = ls.guardarLesion(nueva, ejerciciosIds);
                if (guardado)
                {
                    return Json(new { respuesta = true, mensaje = "Lesión guardada exitosamente." });
                }
                else
                {
                    return Json(new { respuesta = false, mensaje = "Error al guardar la lesión." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult BusquedaLesiones(string busqueda)
        {
            var lesiones = ls.BusaquedaLesiones(busqueda);
            return Json(lesiones, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerTodasLasLesiones()
        {
            var lesiones = ls.ObtenerTodaLesion();
            return Json(lesiones, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerEjerciciosPorCategoria(int idCat)
        {
            var ejercicios = ls.ObtenerEjercicioCategoria(idCat);
            return Json(ejercicios, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerEjerciciosDeLesion(int idLesion)
        {
            var ejercicios = ls.ObtenerEjercicioLesion(idLesion);
            return Json(ejercicios, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActualizarLesion(int id_Lesion, string Nombre_Lesion, string Descrip_Lesion, int id_TipoLesion, string id_Grado, int[] ejerciciosIds)
        {
            try
            {
                Lesion lesionActualizada = new Lesion
                {
                    id_Lesion = id_Lesion,
                    id_TipoLes = id_TipoLesion,
                    Nom_les = Nombre_Lesion,
                    Descrip_Les = Descrip_Lesion,
                    Grado = id_Grado
                };
                bool actualizado = ls.ActualizarLesion(lesionActualizada, ejerciciosIds);
                if (actualizado)
                    return Json(new { respuesta = true, mensaje = "Lesión actualizada exitosamente." });
                else
                    return Json(new { respuesta = false, mensaje = "No se pudo actualizar." });
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, message = ex.Message });
            }

        }

        [HttpPost]
        public ActionResult EliminarLesion(int id_Lesion)
        {
            try
            {
                bool eliminado = ls.EliminarLesion(id_Lesion);

                if (eliminado)
                    return Json(new { respuesta = true, mensaje = "Lesión eliminada exitosamente." });
                else
                    return Json(new { respuesta = false, mensaje = "No se pudo eliminar." });
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, message = ex.Message });
            }
        }
    }
}
