using ExamenWebSalinas.Entidades.InicioSesion;
using ExamenWebSalinas.Negocio.InicioSesion;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ExamenWebSalinas.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index(string message = "")
        {
            ViewBag.Message = message;
            return View();
        }

        [HttpPost]
        public ActionResult Acceso(string usuario, string contrasena)
        {
            if (!string.IsNullOrEmpty(usuario) && !string.IsNullOrEmpty(contrasena))
            {
                SesionLogin datosUsuario = new SesionLogin();
                ProcesaDatosUsuario procesa = new ProcesaDatosUsuario();

                datosUsuario = procesa.ObtenerUsuario(usuario, contrasena);

                if (datosUsuario != null)
                {
                    if (datosUsuario.UsuarioLogin != null)
                    {
                        FormsAuthentication.SetAuthCookie(datosUsuario.UsuarioLogin, true);
                        if (Request.Cookies["cookie_rol"] != null)
                        {
                            HttpCookie myCookie = new HttpCookie("cookie_rol");
                            myCookie.Expires = DateTime.Now.AddDays(-1d);
                            Response.Cookies.Add(myCookie);
                        }
                       
                        HttpCookie cookie1 = new HttpCookie("cookie_rol", datosUsuario.UsuarioLogin);
                        ControllerContext.HttpContext.Response.SetCookie(cookie1);
                        
                        return RedirectToAction("Index","AdministracionUsuarios");
                    }
                    else
                    {
                        return RedirectToAction("Index", new { message = "Usuario y/o contraseña incorrectos" });
                    }

                }
                else
                {
                    return RedirectToAction("Index", new { message = "Usuario y/o contraseña incorrectos" });
                }
            }
            else
            {
                return RedirectToAction("Index", new { message = "Ingresa usuario y/o contraseña" });
            }

        }


        //[Authorize]
        public ActionResult Logout()
        {

            if (Request.Cookies["cookie_rol"] != null)
            {
                HttpCookie myCookie = new HttpCookie("cookie_rol");
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);
            }
            Session.Abandon();

            FormsAuthentication.SignOut();

            return RedirectToAction("Index");

        }


    }
}
