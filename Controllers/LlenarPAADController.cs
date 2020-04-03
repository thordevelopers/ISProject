using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.Mvc;
using ISProject.Models;

namespace ISProject.Controllers
{
    public class LlenarPAADController : Controller
    {
        // GET: PAAD
        public ActionResult Index()
        {
            if (Session["user"] != null)
            {
                LlenarPAADCLS model = new LlenarPAADCLS();
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    PAADs paad = db.PAADs.Where(p => p.docente == doc.id_docentes).First();
                    model.id_paad = paad.id_paad;
                    model.estado = db.Estados.Where(p => p.id_estado == paad.estado).First().estado;
                    model.periodo = db.Periodos.Where(p => p.id_periodo == paad.periodo).First().periodo;
                    model.carrera = db.Carreras.Where(p => p.id_carrera == paad.carrera).First().carrera;
                    model.numero_empleado = doc.numero_empleado;
                    model.nombre_docente = doc.nombre;
                    model.categoria_docente = db.Categorias.Where(p => p.id_categoria == paad.categoria_docente).First().categoria;
                    model.horas_clase = paad.horas_clase;
                    model.horas_investigacion = paad.horas_investigacion;
                    model.horas_gestion = paad.horas_gestion;
                    model.horas_tutorias = paad.horas_tutorias;
                    model.cargo = db.Cargos.Where(p => p.id_cargo == paad.cargo).First().cargo;
                    model.firma_director = paad.firma_director;
                    model.firma_docente = paad.firma_docente;
                    model.modal_open = true;
                    model.activities = db.Actividades.Where(p => p.id_paad == paad.id_paad).Select(x => new ActivityCLS {
                        Id = x.id_actividad,
                        actividad = x.actividad,
                        produccion = x.produccion,
                        lugar = x.lugar,
                        porcentaje_inicial= x.porcentaje_inicial,
                        porcentaje_final=x.porcentaje_final,
                        cacei=x.cacei,
                        cuerpo_academico=x.cuerpo_academico,
                        iso=x.iso
                    }).ToList();
                }
                return View("~/Views/PAAD/PAAD.cshtml",model);
            }
            else
                return View("~/Views/Login/Login");
        }

        [HttpPost]
        public ActionResult PostActivity(LlenarPAADCLS model)
        {
            if (!ModelState.IsValid)
            {
                return View("~/Views/PAAD/PAAD.cshtml");
            }
            else
            {
                ActivityCLS act = model.activity;
                if (model.activity.Id != 0)
                {
                    using (var db = new DB_PAAD_IADEntities())
                    {
                        Actividades act_db = db.Actividades.Single(p => p.id_actividad == model.activity.Id);
                        act_db.actividad = act.actividad;
                        act_db.actividad = act.actividad;
                        act_db.produccion = act.produccion;
                        act_db.lugar = act.lugar;
                        act_db.porcentaje_inicial = act.porcentaje_inicial;
                        act_db.cacei = act.cacei;
                        act_db.cuerpo_academico = act.cuerpo_academico;
                        act_db.iso = act.iso;
                        db.SaveChanges();
                    }
                } 
                else
                {
                    using (var db = new DB_PAAD_IADEntities())
                    {
                        db.Actividades.Add(new Actividades
                        {
                            actividad = act.actividad,
                            produccion = act.produccion,
                            lugar = act.lugar,
                            porcentaje_inicial = act.porcentaje_inicial,
                            cacei = act.cacei,
                            cuerpo_academico = act.cuerpo_academico,
                            iso = act.iso,
                            id_paad = act.id_paad
                        });
                        db.SaveChanges();
                    }
                }
            }
            return View("~/Views/PAAD/PAAD.cshtml",model);
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