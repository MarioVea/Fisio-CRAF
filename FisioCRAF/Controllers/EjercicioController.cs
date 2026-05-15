using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FisioCRAF.Controllers
{
    public class EjercicioController : Controller
    {
        EjercicioService es = new EjercicioService();

        // GET: Ejercicio
        public ActionResult Ejercicio()
        {
            ViewBag.Categorias = es.ObtenerCategorias();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GuardarEjercicio(string Nombre_Ejer, string Descrip_Ejer, int id_CatEjer, HttpPostedFileBase ImagenReferencia)
        {
            try
            {
                string rutaImagen = null;

                //genera la ruta de la imagen 
                if (ImagenReferencia != null && ImagenReferencia.ContentLength > 0)
                {
                    string extension = Path.GetExtension(ImagenReferencia.FileName);
                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaCarpeta = Server.MapPath("~/Content/ImagenesEjercicios/");

                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                    ImagenReferencia.SaveAs(rutaCompleta);

                    //Genera la ruta para la base de datos
                    rutaImagen = "/Content/ImagenesEjercicios/" + nombreArchivo;
                }

                //llenar la clase
                Ejercicio nuevoEjercicio = new Ejercicio
                {
                    Nombre_Ejer = Nombre_Ejer,
                    Descrip_Ejer = Descrip_Ejer,
                    id_CatEjer = id_CatEjer,
                    Imag_Ejer = rutaImagen
                };

                //guardar en la base de datos
                bool guardado = es.guardarEjercicio(nuevoEjercicio);

                if (guardado)
                {
                    return Json(new { respuesta = true, mensaje = "Ejercicio guardado exitosamente." });
                }
                else
                {
                    return Json(new { respuesta = false, mensaje = "Ocurrio un error al guardar." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }

        [HttpGet]
        public ActionResult BusquedaEjercicios(string busqueda)
        {
            var ejercicios = es.busquedaEjercicios(busqueda);
            return Json(ejercicios, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ActualizarEjercicio(int id_Ejercicio, string Nombre_Ejer, string Descrip_Ejer, int id_CatEjer, HttpPostedFileBase ImagenReferencia, string ImagenActual)
        {
            try
            {
                string rutaImagen = ImagenActual;

                if (ImagenReferencia != null && ImagenReferencia.ContentLength > 0)
                {
                    string extension = Path.GetExtension(ImagenReferencia.FileName);
                    string nombreArchivo = Guid.NewGuid().ToString() + extension;
                    string rutaCarpeta = Server.MapPath("~/Content/ImagenesEjercicios/");

                    if (!Directory.Exists(rutaCarpeta))
                    {
                        Directory.CreateDirectory(rutaCarpeta);
                    }

                    string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);
                    ImagenReferencia.SaveAs(rutaCompleta);
                    rutaImagen = "/Content/ImagenesEjercicios/" + nombreArchivo;
                }

                Ejercicio ejercicio = new Ejercicio
                {
                    id_Ejercicio = id_Ejercicio,
                    Nombre_Ejer = Nombre_Ejer,
                    Descrip_Ejer = Descrip_Ejer,
                    id_CatEjer = id_CatEjer,
                    Imag_Ejer = rutaImagen
                };

                bool actualizado = es.actualizarEjercicio(ejercicio);

                if (actualizado)
                {
                    return Json(new { respuesta = true, mensaje = "Ejercicio actualizado exitosamente." });
                }
                else
                {
                    return Json(new { respuesta = false, mensaje = "No se pudo actualizar el ejercicio." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EliminarEjercicio(int id_Ejercicio)
        {
            try
            {
                bool eliminado = es.eliminarEjercicio(id_Ejercicio);

                if (eliminado)
                {
                    return Json(new { respuesta = true, mensaje = "Ejercicio eliminado exitosamente." });
                }
                else
                {
                    return Json(new { respuesta = false, mensaje = "No se pudo eliminar el ejercicio." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }
    }
}