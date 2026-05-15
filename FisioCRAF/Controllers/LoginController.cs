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
        loginService ls = new loginService();



        // GET: Login
        public async Task<ActionResult> Login()
        {
            return View();
        }


        public async Task<ActionResult> iniciar(Models.Entidades.Login login)
        {

            try
            {
                var respuesta = ls.iniciarSesion(login.usuario,login.password);
                return Json(new { respuesta = respuesta});
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return Json(new { message = "Ocurrió un error al iniciar sesion, intentalo más tarde" });
            }

        }




    }
}