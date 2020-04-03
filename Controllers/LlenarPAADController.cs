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
                LlenarPAADCLS model = FillPAAD();
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
            return RedirectToAction("Index", "LlenarPAAD");
        }
        public ActionResult DeleteActivity(int id)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                Actividades act_db = db.Actividades.Single(p => p.id_actividad == id);
                db.Actividades.Remove(act_db);
                db.SaveChanges();
            }
            return RedirectToAction("Index", "LlenarPAAD");
        }

        public ActionResult DeleteAllActivitys(int id)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                List<Actividades> act_db = db.Actividades.Where(p => p.id_paad == id).ToList();
                foreach(var item in act_db){
                    db.Actividades.Remove(item);
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index", "LlenarPAAD");
        }

        public LlenarPAADCLS FillPAAD()
        {

            LlenarPAADCLS model = new LlenarPAADCLS();
            using (var db = new DB_PAAD_IADEntities())
            {
                Docentes doc = ((Docentes)Session["user"]);
                PAADs paad = db.PAADs.Where(p => p.docente == doc.id_docentes).FirstOrDefault();
                if (paad == null)
                {
                    db.PAADs.Add(new PAADs
                    {
                        id_paad = 0,
                        estado = 1,
                        periodo = 1,
                        carrera = 1,
                        docente = doc.id_docentes,
                        categoria_docente = 1,
                        horas_clase = 10,
                        horas_investigacion = 10,
                        horas_gestion = 10,
                        horas_tutorias = 10,
                        cargo = 1
                    });
                    db.SaveChanges();
                }
                paad = db.PAADs.Where(p => p.docente == doc.id_docentes).FirstOrDefault();
                model.id_paad = paad.id_paad;
                model.estado = db.Estados.Where(p => p.id_estado == paad.estado).FirstOrDefault().estado;
                model.periodo = db.Periodos.Where(p => p.id_periodo == paad.periodo).FirstOrDefault().periodo;
                model.carrera = db.Carreras.Where(p => p.id_carrera == paad.carrera).FirstOrDefault().carrera;
                model.numero_empleado = doc.numero_empleado;
                model.nombre_docente = doc.nombre;
                model.categoria_docente = db.Categorias.Where(p => p.id_categoria == paad.categoria_docente).FirstOrDefault().categoria;
                model.horas_clase = paad.horas_clase;
                model.horas_investigacion = paad.horas_investigacion;
                model.horas_gestion = paad.horas_gestion;
                model.horas_tutorias = paad.horas_tutorias;
                model.cargo = db.Cargos.Where(p => p.id_cargo == paad.cargo).FirstOrDefault().cargo;
                model.firma_director = paad.firma_director;
                model.firma_docente = paad.firma_docente;
                model.modal_open = true;
                model.activities = db.Actividades.Where(p => p.id_paad == paad.id_paad).Select(x => new ActivityCLS
                {
                    Id = x.id_actividad,
                    actividad = x.actividad,
                    produccion = x.produccion,
                    lugar = x.lugar,
                    porcentaje_inicial = x.porcentaje_inicial,
                    porcentaje_final = x.porcentaje_final,
                    cacei = x.cacei,
                    cuerpo_academico = x.cuerpo_academico,
                    iso = x.iso
                }).ToList();

            }
            return model;
        }

    }
}