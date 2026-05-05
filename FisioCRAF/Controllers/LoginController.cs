using FisioCRAF.Models.Services;
using FisioCRAF.Models.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FisioCRAF.Controllers
{
    public class LoginController : Controller
    {
        // TODO: Descomentar cuando tengas el archivo de conexión
        // loginService ls = new loginService();



        // GET: Login
        public async Task<ActionResult> Login()
        {
            return View();
        }


        public async Task<ActionResult> iniciar(Models.Entidades.Login login)
        {

            try
            {
                // TODO: Descomentar cuando tengas el archivo de conexión
                // var respuesta = ls.iniciarSesion(login.usuario,login.password);
                var respuesta = "Inicio de sesión exitoso (modo prueba)";
                return Json(new { message = respuesta});
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = "Ocurrió un error al iniciar sesion, intentalo más tarde" });
            }

        }




    }
}