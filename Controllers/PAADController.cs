using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;

namespace ISProject.Controllers
{
    public class PAADController : Controller
    {
        // GET: PAAD
        public ActionResult Index()
        {
            System.Diagnostics.Debug.WriteLine("sesion.id: "+Session["id"]);
            UserCLS id = new UserCLS();
            id.Id =(int) Session["id"];
            return View(id);
        }
    }
}