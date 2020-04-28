using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;


namespace ISProject.Controllers
{
    public class vistaPAADsController : Controller
    {
        // GET: vistaPAADs
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                userPAADs(((Docentes)Session["user"]).id_docentes);
                return View("~/Views/PAAD/vistaPAADs.cshtml");
            }
            else
            {
                return View("~/Views/Login/Login");
            }
        }

        [HttpGet]
        public ActionResult userPAADs(int id)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            else
            {
                List<PAADs> _paads;
                using (var db = new DB_PAAD_IADEntities())
                {
                    _paads = db.PAADs.Where(p => p.docente == id).ToList();
                }
                if (_paads.Count == 0)
                {
                    return View();
                }
                else
                {
                    Session["user_paads"] = _paads;
                }
                _paads = null;
            }
            return View("~/Views/PAAD/vistaPAADs.cshtml");
        }

        public ActionResult GenerarPDF()
        {
            return View();
        }
    }
}