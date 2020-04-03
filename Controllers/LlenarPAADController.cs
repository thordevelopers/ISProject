using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;

namespace ISProject.Controllers
{
    public class LlenarPAADController : Controller
    {
        // GET: PAAD
        public ActionResult Index()
        {
            if (Session["email"] != null)
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    
                }

                    return View("~/Views/PAAD/PAAD.cshtml");
            }
            else
                return View("~/Views/Login/Login");
        }

        [HttpPost]
        public ActionResult PostActivity(ActivityCLS act)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/PAAD/PAAD.cshtml");
            }
            else
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    db.Actividades.Add(new Actividades {
                        actividad = act.actividad,
                        produccion = act.produccion,
                        lugar = act.lugar,
                        porcentaje_inicial = act.porcentaje_inicial,
                        cacei = act.cacei,
                        cuerpo_academico = act.cuerpo_academico,
                        iso = act.iso,
                        id_paad = 1
                    });
                    db.SaveChanges();
                }
            }
            return View("~/Views/PAAD/PAAD.cshtml");
        }

        public ActionResult SelectPercentage()
        {
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "25", Value = "25" });
            items.Add(new SelectListItem { Text = "50", Value = "50" });
            items.Add(new SelectListItem { Text = "75", Value = "75" });
            items.Add(new SelectListItem { Text = "100", Value = "100" });
            ViewBag.PAAD = items;
            return View();
        }
    }
}