using FisioCRAF.Models.Entidades;
using FisioCRAF.Models.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FisioCRAF.Controllers
{
    public class DiagnosticoController : Controller
    {
        DiagnosticoService ds = new DiagnosticoService();

        // GET: Diagnostico
        public ActionResult Diagnostico()
        {
            ViewBag.Consultorios = ds.ObtenerConsultorios();
            ViewBag.EscalasDolor = ds.ObtenerEscalasDolor();
            ViewBag.Tratamientos = ds.ObtenerTratamientos();
            ViewBag.Lesiones = ds.ObtenerLesiones();
            ViewBag.TipoLesiones = ds.ObtenerTipoLesiones();
            ViewBag.Ejercicios = ds.ObtenerEjercicios();
            ViewBag.Categorias = ds.ObtenerCategorias();
            return View();
        }

        [HttpGet]
        public ActionResult BuscarDiagnosticos(string busqueda)
        {
            var resultados = ds.BuscarDiagnosticos(busqueda);
            return Json(resultados, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ObtenerDetalleDiagnostico(int id)
        {
            var detalle = ds.ObtenerDiagnosticoPorId(id);
            if (detalle == null)
            {
                return Json(new { respuesta = false, mensaje = "No se encontró el diagnóstico." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { respuesta = true, datos = detalle }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GuardarDiagnostico()
        {
            try
            {
                var reader = new StreamReader(Request.InputStream);
                string body = reader.ReadToEnd();
                dynamic datos = JsonConvert.DeserializeObject(body);

                int idCita = (int)datos.id_Cita;
                int? idConsul = datos.id_Consul != null ? (int?)datos.id_Consul : null;

                // Parsear DetallesDiag
                var detallesDiag = new List<DetalleDiag>();
                if (datos.DetallesDiag != null)
                {
                    foreach (var dd in datos.DetallesDiag)
                    {
                        detallesDiag.Add(new DetalleDiag
                        {
                            Tipo_Diag = (bool)dd.Tipo_Diag,
                            Nombre_Diag = (string)dd.Nombre_Diag,
                            id_Lesion = dd.id_Lesion != null ? (int?)dd.id_Lesion : null,
                            RadioGrafia = (string)dd.RadioGrafia,
                            Descrip_Diag = (string)dd.Descrip_Diag,
                            id_EscalaDolor = dd.id_EscalaDolor != null ? (int?)dd.id_EscalaDolor : null
                        });
                    }
                }

                // Parsear DetallesEjer
                var detallesEjer = new List<DetalleEjerDiag>();
                if (datos.DetallesEjer != null)
                {
                    foreach (var de in datos.DetallesEjer)
                    {
                        detallesEjer.Add(new DetalleEjerDiag
                        {
                            id_Ejercicio = de.id_Ejercicio != null ? (int?)de.id_Ejercicio : null,
                            id_DetalleEjerLes = de.id_DetalleEjerLes != null ? (int?)de.id_DetalleEjerLes : null,
                            Series = (byte)de.Series,
                            Repeticiones = (byte)de.Repeticiones
                        });
                    }
                }

                // Parsear DetallesCH
                var detallesCH = new List<DetalleCH>();
                var tratamientoIds = new List<int?>();
                if (datos.DetallesCH != null)
                {
                    foreach (var ch in datos.DetallesCH)
                    {
                        detallesCH.Add(new DetalleCH
                        {
                            Imag_Musculo = (string)ch.Imag_Musculo ?? "",
                            Nombre_Musculo = (string)ch.Nombre_Musculo,
                            Descripcion_DiagCH = (string)ch.Descripcion_DiagCH ?? ""
                        });
                        tratamientoIds.Add(ch.id_Trata != null ? (int?)ch.id_Trata : null);
                    }
                }

                bool guardado = ds.GuardarDiagnostico(idCita, idConsul, detallesDiag, detallesEjer, detallesCH, tratamientoIds);

                if (guardado)
                    return Json(new { respuesta = true, mensaje = "Diagnóstico guardado exitosamente." });
                else
                    return Json(new { respuesta = false, mensaje = "No se pudo guardar el diagnóstico." });
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult ActualizarDiagnostico()
        {
            try
            {
                var reader = new StreamReader(Request.InputStream);
                string body = reader.ReadToEnd();
                dynamic datos = JsonConvert.DeserializeObject(body);

                int idDiag = (int)datos.id_Diag;
                int? idConsul = datos.id_Consul != null ? (int?)datos.id_Consul : null;

                // Parsear DetallesDiag
                var detallesDiag = new List<DetalleDiag>();
                if (datos.DetallesDiag != null)
                {
                    foreach (var dd in datos.DetallesDiag)
                    {
                        detallesDiag.Add(new DetalleDiag
                        {
                            Tipo_Diag = (bool)dd.Tipo_Diag,
                            Nombre_Diag = (string)dd.Nombre_Diag,
                            id_Lesion = dd.id_Lesion != null ? (int?)dd.id_Lesion : null,
                            RadioGrafia = (string)dd.RadioGrafia,
                            Descrip_Diag = (string)dd.Descrip_Diag,
                            id_EscalaDolor = dd.id_EscalaDolor != null ? (int?)dd.id_EscalaDolor : null
                        });
                    }
                }

                // Parsear DetallesEjer
                var detallesEjer = new List<DetalleEjerDiag>();
                if (datos.DetallesEjer != null)
                {
                    foreach (var de in datos.DetallesEjer)
                    {
                        detallesEjer.Add(new DetalleEjerDiag
                        {
                            id_Ejercicio = de.id_Ejercicio != null ? (int?)de.id_Ejercicio : null,
                            id_DetalleEjerLes = de.id_DetalleEjerLes != null ? (int?)de.id_DetalleEjerLes : null,
                            Series = (byte)de.Series,
                            Repeticiones = (byte)de.Repeticiones
                        });
                    }
                }

                // Parsear DetallesCH
                var detallesCH = new List<DetalleCH>();
                var tratamientoIds = new List<int?>();
                if (datos.DetallesCH != null)
                {
                    foreach (var ch in datos.DetallesCH)
                    {
                        detallesCH.Add(new DetalleCH
                        {
                            Imag_Musculo = (string)ch.Imag_Musculo ?? "",
                            Nombre_Musculo = (string)ch.Nombre_Musculo,
                            Descripcion_DiagCH = (string)ch.Descripcion_DiagCH ?? ""
                        });
                        tratamientoIds.Add(ch.id_Trata != null ? (int?)ch.id_Trata : null);
                    }
                }

                bool actualizado = ds.ActualizarDiagnostico(idDiag, idConsul, detallesDiag, detallesEjer, detallesCH, tratamientoIds);

                if (actualizado)
                    return Json(new { respuesta = true, mensaje = "Diagnóstico actualizado exitosamente." });
                else
                    return Json(new { respuesta = false, mensaje = "No se pudo actualizar el diagnóstico." });
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EliminarDiagnostico(int id_Diag)
        {
            try
            {
                bool eliminado = ds.EliminarDiagnostico(id_Diag);

                if (eliminado)
                    return Json(new { respuesta = true, mensaje = "Diagnóstico eliminado exitosamente." });
                else
                    return Json(new { respuesta = false, mensaje = "No se pudo eliminar el diagnóstico." });
            }
            catch (Exception ex)
            {
                return Json(new { respuesta = false, mensaje = ex.Message });
            }
        }
    }
}