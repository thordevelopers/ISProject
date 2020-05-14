using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using Antlr.Runtime.Tree;
using ISProject.Models;
using ISProject.Controllers;
using Microsoft.Ajax.Utilities;
using System.Web.Routing;

namespace ISProject.Controllers
{
    public class DocenteController : Controller
    {
        AuthenticationController auth = new AuthenticationController();
        public ActionResult Home()
        {
            return View("HomeDocente");
        }
        //ModifyPAAD actions
        public ActionResult ModifyPAAD()
        {
            InfoPAADCLS info = GetInfoPAAD();
            if (info != null && info.status_value != 1)
                return RedirectToAction("ViewPAAD", new { id = info.id_paad });
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            ViewBag.msg = GetMessages(info.id_paad);
            return View();
        }
        public ActionResult ModalActivity(int mdl_id_paad, int mdl_id_activity)
        {
            ActivityCLS modal = null;
            if (mdl_id_activity > 0)
            {
                using (var db = new DB_PAAD_IADEntities())
                {
                    modal = (from activity in db.Actividades
                             where activity.id_actividad == mdl_id_activity
                             select new ActivityCLS
                             {
                                 id = activity.id_actividad,
                                 actividad = activity.actividad,
                                 produccion = activity.produccion,
                                 lugar = activity.lugar,
                                 porcentaje_inicial = activity.porcentaje_inicial,
                                 porcentaje_final = activity.porcentaje_final,
                                 cacei = activity.cacei,
                                 cuerpo_academico = activity.cuerpo_academico,
                                 iso = activity.iso,
                                 id_paad = activity.id_paad
                             }).FirstOrDefault();
                }
            }
            else
            {
                modal = new ActivityCLS { id_paad = mdl_id_paad };
            }
            return PartialView("_ModalActivityPAAD",modal);
        }
        [HttpPost]
        public ActionResult SaveActivity(ActivityCLS model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_ModalActivityPAAD", model)
                });
            }
            else
            {
                if (model.id != 0)
                {
                    using (var db = new DB_PAAD_IADEntities())
                    {
                        Actividades act_db = db.Actividades.Single(p => p.id_actividad == model.id);
                        act_db.actividad = model.actividad;
                        act_db.actividad = model.actividad;
                        act_db.produccion = model.produccion;
                        act_db.lugar = model.lugar;
                        act_db.porcentaje_inicial = model.porcentaje_inicial;
                        act_db.cacei = model.cacei;
                        act_db.cuerpo_academico = model.cuerpo_academico;
                        act_db.iso = model.iso;
                        db.SaveChanges();
                    }
                }
                else
                {
                    using (var db = new DB_PAAD_IADEntities())
                    {
                        db.Actividades.Add(new Actividades
                        {
                            actividad = model.actividad,
                            produccion = model.produccion,
                            lugar = model.lugar,
                            porcentaje_inicial = model.porcentaje_inicial,
                            cacei = model.cacei,
                            cuerpo_academico = model.cuerpo_academico,
                            iso = model.iso,
                            id_paad = model.id_paad
                        });
                        db.SaveChanges();
                    }
                }
                List<ActivityCLS> list = GetActivities(model.id_paad);
                return Json(new
                {
                    Status = 1,
                    Message = "Success",
                    AjaxResponse = RenderRazorViewToString("_EditActivitiesTable", list)
                });
            }
        }
        public ActionResult DeleteActivity(int del_id_paad, int del_id_activity)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                Actividades act_db = db.Actividades.Single(p => p.id_actividad == del_id_activity);
                db.Actividades.Remove(act_db);
                db.SaveChanges();
            }
            List<ActivityCLS> list = GetActivities(del_id_paad);
            return PartialView("_EditActivitiesTable", list);
        }
        public ActionResult DropActivities(int drop_id_paad)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                List<Actividades> act_db = db.Actividades.Where(p => p.id_paad == drop_id_paad).ToList();
                foreach (var item in act_db)
                {
                    db.Actividades.Remove(item);
                }
                db.SaveChanges();
            }
            return PartialView("_EditActivitiesTable", new List<ActivityCLS>());
        }
        //ViewPAAD_Docente Actions
        public ActionResult ViewPAAD(int id)
        {
            InfoPAADCLS info = GetInfoPAAD(id);
            if (info == null && info.status_value == 1)
                return RedirectToAction("ModifyPAAD");
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            ViewBag.msg = GetMessages(info.id_paad);
            return View("ViewPAAD_Docente");
        }
        public ActionResult ApplyActionPAAD(AuthenticationCLS credentials,int id_paad,int action_paad,string message_modif=null)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            Docentes doc = ((Docentes)Session["user"]);
            if (!auth.AuthenticateCredentials(credentials.email, credentials.password) || doc.correo != credentials.email)
            {
                credentials.message = "Correo y/o contraseña incorrectos";
                return Json(new
                {
                    Status = 3,
                    Message = "Error",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            }
            using (var db = new DB_PAAD_IADEntities())
            {
                PAADs paad = db.PAADs.Where(p => p.id_paad == id_paad && p.docente == doc.id_docente).FirstOrDefault();
                if (paad == null)
                {
                    credentials.message = "Correo y/o contraseña incorrectos";
                    return Json(new
                    {
                        Status = 3,
                        Message = "Error",
                        AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                    });
                }
                if (action_paad == 1)
                {
                    paad.estado = 2;
                    paad.razones_rechazo = null;
                    paad.firma_docente = Guid.NewGuid().ToString("N");
                }
                else if (action_paad == 2)
                {
                    paad.estado = 1;
                    paad.firma_docente = null;
                }
                else if (action_paad == 3)
                {
                    paad.estado = 4;
                    paad.razones_modificacion = message_modif;
                    paad.razones_rechazo_solicitud = null;
                }
                else if (action_paad == 4)
                {
                    paad.estado = 3;
                }
                db.SaveChanges();
            }
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        //ListRecordPAADs_Docente Actions
        public ActionResult ListRecordPAADs()
        {
            ViewBag.list = GetRecordPAADs();
            ViewBag.periods = GetPeriods();
            return View("ListRecordPAADs_Docente");
        }
        public ActionResult FilterRecordPAADs(string id_period)
        {
            List<RegistroPAAD> list = GetRecordPAADs(Convert.ToInt32(id_period));
            return PartialView("_ListPAADs", list);
        }
        //Utility functions
        public InfoPAADCLS GetInfoPAAD(int id=0)
        {
            InfoPAADCLS info = new InfoPAADCLS();
            Docentes doc = (Docentes)Session["user"];
            using (var db = new DB_PAAD_IADEntities())
            {
                if (id>0)
                    info = (from paad in db.PAADs
                            where paad.id_paad == id
                            join estado in db.Estados
                            on paad.estado equals estado.id_estado
                            join periodo in db.Periodos
                            on paad.periodo equals periodo.id_periodo
                            select new InfoPAADCLS
                            {
                                id_paad = paad.id_paad,
                                status_value = paad.estado,
                                status_name = estado.estado,
                                active=periodo.activo
                            }).FirstOrDefault();
                else
                    info = (from paad in db.PAADs
                            where paad.docente == doc.id_docente
                            join estado in db.Estados
                            on paad.estado equals estado.id_estado
                            join periodo in db.Periodos
                            on paad.periodo equals periodo.id_periodo
                            where periodo.activo==true
                            select new InfoPAADCLS
                            {
                                id_paad = paad.id_paad,
                                status_value = paad.estado,
                                status_name = estado.estado,
                                active = periodo.activo
                            }).FirstOrDefault();
            }
            return info;
        }
        public HeaderPAADCLS GetHeader(int id)
        {
            HeaderPAADCLS header = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                header = (from paads in db.PAADs
                          where paads.id_paad == id
                          join estado in db.Estados
                          on paads.estado equals estado.id_estado
                          join periodo in db.Periodos
                          on paads.periodo equals periodo.id_periodo
                          join carrera in db.Carreras
                          on paads.carrera equals carrera.id_carrera
                          join docente in db.Docentes
                          on paads.docente equals docente.id_docente
                          join categoria in db.Categorias
                          on paads.categoria_docente equals categoria.id_categoria
                          join cargo in db.Cargos
                          on paads.cargo equals cargo.id_cargo
                          select new HeaderPAADCLS
                          {
                              periodo = periodo.periodo,
                              nombre = docente.nombre,
                              numero_empleado = docente.numero_empleado,
                              categoria = categoria.categoria,
                              cargo = cargo.cargo,
                              horas_clase = paads.horas_clase,
                              horas_gestion = paads.horas_gestion,
                              horas_investigacion = paads.horas_investigacion,
                              horas_tutorias = paads.horas_tutorias

                          }).First();

            }
            return header;
        }
        public List<ActivityCLS> GetActivities(int id)
        {
            List<ActivityCLS> activities = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                activities = (from activity in db.Actividades
                              where activity.id_paad == id
                              select new ActivityCLS
                              {
                                  id = activity.id_actividad,
                                  actividad = activity.actividad,
                                  produccion = activity.produccion,
                                  lugar = activity.lugar,
                                  porcentaje_inicial = activity.porcentaje_inicial,
                                  porcentaje_final = activity.porcentaje_final,
                                  cacei = activity.cacei,
                                  cuerpo_academico = activity.cuerpo_academico,
                                  iso = activity.iso,
                                  id_paad = activity.id_paad
                              }).ToList();
            }
            return activities;
        }
        public List<RegistroPAAD> GetRecordPAADs(int period=0)
        {
            List<RegistroPAAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                Docentes doc = ((Docentes)Session["user"]);
                list = (from paad in db.PAADs
                        where paad.docente == doc.id_docente 
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        where period > 0 ? periodo.id_periodo == period : true
                        join carrera in db.Carreras
                        on paad.carrera equals carrera.id_carrera
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        join categoria in db.Categorias
                        on paad.categoria_docente equals categoria.id_categoria
                        join cargo in db.Cargos
                        on paad.cargo equals cargo.id_cargo
                        select new RegistroPAAD
                        {
                            id_paad = paad.id_paad,
                            estado = estado.estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre,
                            categoria_docente = categoria.categoria,
                            cargo = cargo.cargo
                        }).ToList();
            }
            return list;
        }
        public List<SelectListItem> GetPeriods()
        {
            List<SelectListItem> periods=null;
            using(var db = new DB_PAAD_IADEntities())
            {
                periods = (from periodo in db.Periodos
                           select new SelectListItem
                           {
                               Text = periodo.periodo,
                               Value = periodo.id_periodo.ToString()
                           }).ToList();
                periods.Insert(0, new SelectListItem { Text = "Todos", Value = "0" });
            }
            return periods;
        }
        public MessagesPAADCLS GetMessages(int id)
        {
            MessagesPAADCLS msg;
            using(var db=new DB_PAAD_IADEntities())
            {
                msg = (from paad in db.PAADs
                       where paad.id_paad == id
                       select new MessagesPAADCLS
                       {
                           reject_paad = paad.razones_rechazo,
                           request_modificaction = paad.razones_modificacion,
                           reject_modificaction = paad.razones_rechazo_solicitud
                       }).FirstOrDefault();
            }
            return msg;
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