
using ExamenWebSalinas.Entidades.ControlUsuarios;
using ExamenWebSalinas.Negocio.InicioSesion;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ExamenWebSalinas.Controllers
{
    [Authorize]
    public class AdministracionUsuariosController : Controller
    {
        // GET: AdministracionUsuarios
        public ActionResult Index()
        {
            return View();
        }

        
        public PartialViewResult ListaUsuarios()
        {
            ModelState.Clear();

            List<Usuarios> usuarios = new List<Usuarios>();
            ProcesaDatosUsuario procesa = new ProcesaDatosUsuario();

            usuarios = procesa.ObtenerUsuarios();
   
            return PartialView("_ListaUsuarios", usuarios);
        }

        [HttpPost]
        public ActionResult ListaUsuariosRecarga()
        {

            ModelState.Clear();

            List<Usuarios> usuarios = new List<Usuarios>();
            ProcesaDatosUsuario procesa = new ProcesaDatosUsuario();

            usuarios = procesa.ObtenerUsuarios();

            return PartialView("_ListaUsuarios", usuarios);
        }


        [HttpPost]
        public ActionResult EliminarUsuario(int IdUsuario)
        {
            ProcesaDatosUsuario procesa = new ProcesaDatosUsuario();
            bool respuesta = procesa.EliminarUsuario(IdUsuario);
       
            return Json(respuesta);
        }

        [HttpPost]
        public ActionResult ActualizarUsuario(Usuarios model)
        {
            ProcesaDatosUsuario procesa = new ProcesaDatosUsuario();
            bool respuesta = procesa.ActualizarUsuario(model);

            return Json(respuesta);
        }

        

        [HttpPost]
        public ActionResult RegistrarUsuario(Usuarios model)
        {
            ProcesaDatosUsuario procesa = new ProcesaDatosUsuario();
            bool respuesta = procesa.RegistrarUsuario(model);

            return Json(respuesta);
        }

    }
}
