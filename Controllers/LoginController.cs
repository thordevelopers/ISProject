using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;

namespace ISProject.Controllers
{
    public class LoginController : Controller
    {
        /*Se inicializa un auxiliar para las funciones de aunteticacion, mas detalles sobre estas funciones las puedes encontrar en el controlador "AuthenticationController" */
        AuthenticationController auth = new AuthenticationController();
        //Acciones de la vista ------------------------------------------------ HomeSubdirector ------------------------------------------------
        //Esta vista muestra la vista de Login
        public ActionResult Login()
        {
            return View();
        }
        /* Esta accion se llama cuando se presiona el boton de iniciar sesion
         * Esta accion valida las credenciales para crear un sesion del usuario
         * Recibe la credenciales
         * Regresa un json con el estado de la respuesta, el mensaje de respuesta, y una vista parcial en string */
        [HttpPost]
        public ActionResult Login(AuthenticationCLS credentials)
        {
            //Valida los campos del modelo de las credenciales
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            //Valida que la autenticacion sea correcta
            if (!auth.AuthenticateCredentials(credentials.email, credentials.password))
            {
                credentials.message = "Correo y/o contraseña incorrectos";
                return Json(new
                {
                    Status = 3,
                    Message = "Error",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            }
            USERS user_db;
            Docentes doc;
            //obtiene el usario segun la contrasena y el correo
            using (var db= new DB_PAAD_IADEntities())
            {
                user_db =  db.USERS.Where(p => p.EMAIL== credentials.email && p.PASSWORD==credentials.password).FirstOrDefault();
            }
            //Obtiene al docente segun el correo
            using (var db = new DB_PAAD_IADEntities())
            {
                doc = db.Docentes.Where(p => p.correo == user_db.EMAIL).FirstOrDefault();
            }
            //Crea una session de usuario
            Session["user"] = doc;
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        /* Esta accion se llama cuando login se ha ejecutado correctamente
         * Esta accion redirige al home correspondiente del usuario
         * No recibe argumentos
         * Regresa una redireccion hacia una accion*/
        public ActionResult RedirectToHome()
        {
            Docentes doc = (Docentes)Session["user"];
            if (doc==null)
                return RedirectToAction("Login");
            if (doc.rol == 1)
                return RedirectToAction("Home", "Docente");
            else if (doc.rol == 2)
                return RedirectToAction("Home", "Coordinador");
            else if (doc.rol == 3)
                return RedirectToAction("Home", "Subdirector");
            else if (doc.rol == 4)
                return RedirectToAction("Home", "Director");
            return RedirectToAction("Login");
        }
        /* Esta accion se llama con el boton de cerrar session en el menu
         * Esta accion borra la session del usuario
         * No recibe parametros
         * Regresa una redireccion a la accion login*/
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
        //Funciones de  ------------------------------------------------ Utilidades ------------------------------------------------
        /* Esta accion transforma una vista en string
         * Recibe el nombre de la vista y el modelo con el cual llenar la vista
         * Regresa un string con la vista 
         * Esta funcion fue obtenida de stackoverflow: https://stackoverflow.com/questions/17554734/mvc-render-partialviewresult-to-string */
        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext,
                                                                         viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View,
                                             ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}