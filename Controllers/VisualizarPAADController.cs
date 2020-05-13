using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ISProject.Models;
using Rotativa;

namespace ISProject.Controllers
{
    public class VisualizarPAADController : Controller
    {
        // GET: VisualizarPAAD
        public ActionResult ViewPAAD(int id)
        {
            VisualizarPAADCLS paad = FillPAAD(id);
            return View(paad);
        }

        public ActionResult ViewPDF(int id)
        {
            VisualizarPAADCLS paad = FillPAAD(id);
            return new ViewAsPdf(paad)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape
            };
        }

        public ActionResult ListPAADs()
        {
            if (Session["user"] != null)
            {
                List<RegistroPAAD> paads = userPAADs();
                return View(paads);
            }
            else
            {
                return RedirectToAction("Login", "Login");
            }
        }

        public VisualizarPAADCLS FillPAAD(int id)
        {
            VisualizarPAADCLS model = new VisualizarPAADCLS();
            using (var db = new DB_PAAD_IADEntities())
            {
                Docentes doc = ((Docentes)Session["user"]);
                PAADs paad;
                if (doc.rol > 1)
                    paad = db.PAADs.Where(p => p.id_paad == id).FirstOrDefault();
                else
                    paad = db.PAADs.Where(p => p.docente == doc.id_docente && p.id_paad == id).FirstOrDefault();
                if (paad == null)
                {
                    return null;
                }
                model.id_paad = paad.id_paad;
                model.estado = db.Estados.Where(p => p.id_estado == paad.estado).FirstOrDefault().estado;
                model.periodo = db.Periodos.Where(p => p.id_periodo == paad.periodo).FirstOrDefault().periodo;
                model.carrera = db.Carreras.Where(p => p.id_carrera == paad.carrera).FirstOrDefault().carrera;
                model.numero_empleado = db.Docentes.Where(p => p.id_docente == paad.docente).FirstOrDefault().numero_empleado;
                model.nombre_docente = db.Docentes.Where(p => p.id_docente == paad.docente).FirstOrDefault().nombre;
                model.categoria_docente = db.Categorias.Where(p => p.id_categoria == paad.categoria_docente).FirstOrDefault().categoria;
                model.horas_clase = paad.horas_clase;
                model.horas_investigacion = paad.horas_investigacion;
                model.horas_gestion = paad.horas_gestion;
                model.horas_tutorias = paad.horas_tutorias;
                model.cargo = db.Cargos.Where(p => p.id_cargo == paad.cargo).FirstOrDefault().cargo;
                model.firma_director = paad.firma_director;
                model.firma_docente = paad.firma_docente;
                model.activities = db.Actividades.Where(p => p.id_paad == paad.id_paad).Select(x => new ActivityCLS
                {
                    id = x.id_actividad,
                    actividad = x.actividad,
                    produccion = x.produccion,
                    lugar = x.lugar,
                    porcentaje_inicial = x.porcentaje_inicial,
                    porcentaje_final = x.porcentaje_final,
                    cacei = x.cacei,
                    cuerpo_academico = x.cuerpo_academico,
                    iso = x.iso
                }).ToList();
                model.rol = doc.rol;

            }
            return model;
        }

        [HttpGet]
        public List<RegistroPAAD> userPAADs()
        {
            RegistroPAAD model;
            List<RegistroPAAD> list = new List<RegistroPAAD>();
            using (var db = new DB_PAAD_IADEntities())
            {
                Docentes doc = ((Docentes)Session["user"]);
                List<PAADs> paads;
                if (doc.rol == 1)
                    paads = db.PAADs.Where(p => p.docente == doc.id_docente).ToList();
                else if (doc.rol == 2)
                    paads = db.PAADs.Where(p => p.estado != 1).ToList();
                else if (doc.rol == 3)
                    paads = db.PAADs.Where(p => p.estado != 1).ToList();
                else
                    paads = null;
                if (paads == null)
                {
                    return list;
                }
                foreach (var paad in paads)
                {
                    model = new RegistroPAAD();
                    model.id_paad = paad.id_paad;
                    model.estado = db.Estados.Where(p => p.id_estado == paad.estado).FirstOrDefault().estado;
                    model.periodo = db.Periodos.Where(p => p.id_periodo == paad.periodo).FirstOrDefault().periodo;
                    model.carrera = db.Carreras.Where(p => p.id_carrera == paad.carrera).FirstOrDefault().carrera;
                    model.numero_empleado = db.Docentes.Where(p => p.id_docente == paad.docente).FirstOrDefault().numero_empleado;
                    model.nombre_docente = db.Docentes.Where(p => p.id_docente == paad.docente).FirstOrDefault().nombre;
                    model.categoria_docente = db.Categorias.Where(p => p.id_categoria == paad.categoria_docente).FirstOrDefault().categoria;
                    model.cargo = db.Cargos.Where(p => p.id_cargo == paad.cargo).FirstOrDefault().cargo;
                    list.Add(model);
                }
            }
            return list;
        }
        [HttpPost]
        public ActionResult doAction(VisualizarPAADCLS model)
        {
            if (model.action == "Cancel")
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    if (db.USERS.Where(p => p.EMAIL == model.user.Email && p.PASSWORD == model.user.Password && p.EMAIL == doc.correo) == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    PAADs paad = db.PAADs.Where(p => p.id_paad == model.id_paad).FirstOrDefault();
                    if (paad == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    paad.estado = 1;
                    db.SaveChanges();
                }
                return RedirectToAction("Index", "LlenarPAAD");
            } 
            else if (model.action == "Request")
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    if (db.USERS.Where(p => p.EMAIL == model.user.Email && p.PASSWORD == model.user.Password && p.EMAIL == doc.correo) == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    PAADs paad = db.PAADs.Where(p => p.id_paad == model.id_paad).FirstOrDefault();
                    if (paad == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    paad.estado = 4;
                    db.SaveChanges();
                }
                return RedirectToAction("ListPAADs", "VisualizarPAAD");
            }
            else if (model.action == "Reject")
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    if (db.USERS.Where(p => p.EMAIL == model.user.Email && p.PASSWORD == model.user.Password && p.EMAIL == doc.correo) == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    PAADs paad = db.PAADs.Where(p => p.id_paad == model.id_paad).FirstOrDefault();
                    if (paad == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    paad.estado = 1;
                    db.SaveChanges();
                }
                return RedirectToAction("ListPAADs", "VisualizarPAAD");
            }
            else if (model.action == "Approve")
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    if (db.USERS.Where(p => p.EMAIL == model.user.Email && p.PASSWORD == model.user.Password && p.EMAIL == doc.correo) == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    PAADs paad = db.PAADs.Where(p => p.id_paad == model.id_paad).FirstOrDefault();
                    if (paad == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    paad.estado = 3;
                    db.SaveChanges();
                }
                return RedirectToAction("ListPAADs", "VisualizarPAAD");
            }
            else if (model.action == "RejReq")
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    if (db.USERS.Where(p => p.EMAIL == model.user.Email && p.PASSWORD == model.user.Password && p.EMAIL == doc.correo) == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    PAADs paad = db.PAADs.Where(p => p.id_paad == model.id_paad).FirstOrDefault();
                    if (paad == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    paad.estado = 3;
                    db.SaveChanges();
                }
                return RedirectToAction("ListPAADs", "VisualizarPAAD");
            }
            else if (model.action == "Modif")
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    Docentes doc = ((Docentes)Session["user"]);
                    if (db.USERS.Where(p => p.EMAIL == model.user.Email && p.PASSWORD == model.user.Password && p.EMAIL == doc.correo) == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    PAADs paad = db.PAADs.Where(p => p.id_paad == model.id_paad).FirstOrDefault();
                    if (paad == null)
                        return RedirectToAction("ListPAADs", "VisualizarPAAD");
                    paad.estado = 1;
                    db.SaveChanges();
                }
                return RedirectToAction("ListPAADs", "VisualizarPAAD");
            }
            return RedirectToAction("ListPAADs", "VisualizarPAAD");
        }
    }
}