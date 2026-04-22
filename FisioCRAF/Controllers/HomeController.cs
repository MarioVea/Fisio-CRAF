using FisioCRAF.Models;
using FisioCRAF.Models.Services;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FisioCRAF.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            PacienteService ps = new PacienteService();
            Paciente paciente = ps.obtenerPaciente(1);
            int id = paciente.id_Pac;
            return View(paciente);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}