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
        AuthenticationController auth = new AuthenticationController();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AuthenticationCLS credentials)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
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
            using (var db= new DB_PAAD_IADEntities())
            {
                user_db =  db.USERS.Where(p => p.EMAIL== credentials.email && p.PASSWORD==credentials.password).FirstOrDefault();
            }
            using (var db = new DB_PAAD_IADEntities())
            {
                doc = db.Docentes.Where(p => p.correo == user_db.EMAIL).FirstOrDefault();
            }
            Session["user"] = doc;
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
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
        public ActionResult LogOut()
        {
            Session.Abandon();
            return RedirectToAction("Login");
        }
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