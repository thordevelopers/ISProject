using ISProject.Filters;
using ISProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Controllers
{
    [FilterSubdirector]
    public class SubdirectorController : Controller
    {
        AuthenticationController auth = new AuthenticationController();
        public ActionResult Home()
        {
            return View("HomeSubdirector");
        }
        // ViewPAAD Actions
        public ActionResult ViewPAAD(int id)
        {
            if (id < 1)
                return RedirectToAction("Home");
            InfoPAADCLS info = GetInfoPAAD(id);
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            if (info.isdirector)
                ViewBag.msg = GetMessages(info.id_paad);
            else
                ViewBag.msg = new MessagesPAADCLS();
            return View("ViewPAAD_Subdirector");
        }
        public ActionResult ApplyActionPAAD(AuthenticationCLS credentials, int id_paad, int action_paad, string reject_message)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            Docentes doc = ((Docentes)Session["user"]);
            if (!auth.AuthenticateCredentials(credentials.email, credentials.password) || doc.rol <3)
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
                PAADs paad = db.PAADs.Where(p => p.id_paad == id_paad).FirstOrDefault();
                bool isdirector = (from docente in db.Docentes where docente.id_docente == paad.docente select docente.isdirector).FirstOrDefault();
                if (paad == null || !isdirector)
                {
                    credentials.message = "Correo y/o contraseña incorrectos";
                    return Json(new
                    {
                        Status = 3,
                        Message = "Error",
                        AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                    });
                }
                else if (action_paad == 1)
                {
                    paad.estado = 1;
                    paad.firma_docente = null;
                    paad.razones_rechazo = reject_message;
                }
                else if (action_paad == 2)
                {
                    paad.estado = 3;
                    paad.firma_director = Guid.NewGuid().ToString("N");
                }
                else if (action_paad == 3)
                {
                    paad.estado = 3;
                    paad.razones_rechazo_solicitud = reject_message;
                    paad.razones_modificacion = null;
                }
                else if (action_paad == 4)
                {
                    paad.estado = 1;
                    paad.firma_docente = null;
                    paad.firma_director = null;
                    paad.razones_rechazo_solicitud = null;
                    paad.razones_modificacion = null;
                    paad.razones_rechazo = null;
                }
                db.SaveChanges();
            }
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        // ListActivePAADs Actions
        public ActionResult ListActivePAADs()
        {
            ViewBag.list = GetActivePAADs();
            ViewBag.states = GetStates();
            ViewBag.careers = GetCareers();
            return View("ListActivePAADs_Subdirector");
        }
        public ActionResult FilterActivePAADs(string filter_state, string filter_career)
        {
            List<RegistroPAAD> list = GetActivePAADs(Convert.ToInt32(filter_state), Convert.ToInt32(filter_career));
            return PartialView("_ListPAADs", list);
        }
        //ListRecordPAADs Actions
        public ActionResult ListRecordPAADs()
        {
            ViewBag.list = GetRecordPAADs();
            ViewBag.period = GetPeriods();
            ViewBag.careers = GetCareers();
            return View("ListRecordPAADs_Subdirector");
        }
        public ActionResult FilterRecordPAADs(string filter_period, string filter_career)
        {
            List<RegistroPAAD> list = GetRecordPAADs(Convert.ToInt32(filter_period), Convert.ToInt32(filter_career));
            return PartialView("_ListPAADs", list);
        }
        // Utilities actions 
        public InfoPAADCLS GetInfoPAAD(int id)
        {
            InfoPAADCLS info = new InfoPAADCLS();
            Docentes doc = (Docentes)Session["user"];
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from paad in db.PAADs
                        where paad.id_paad == id
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        select new InfoPAADCLS
                        {
                            id_paad = paad.id_paad,
                            status_value = paad.estado,
                            status_name = estado.estado,
                            active = periodo.activo,
                            isdirector = docente.isdirector
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
        public List<RegistroPAAD> GetActivePAADs(int state = 0, int career = 0)
        {
            List<RegistroPAAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from docente in db.Docentes
                        where docente.rol==1
                        join paad in db.PAADs
                        on docente.id_docente equals paad.docente into gpaad
                        from paad in gpaad.DefaultIfEmpty()
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado into gestado
                        from estado in gestado.DefaultIfEmpty()
                        where state > 0 && !(estado == null && state == 1) ? estado.id_estado == state && estado.id_estado != 3 : estado.id_estado != 3
                        join carrera in db.Carreras
                        on docente.carrera equals carrera.id_carrera
                        where career > 0 ? carrera.id_carrera == career : true
                        from periodo in db.Periodos
                        where periodo.activo == true
                        select new RegistroPAAD
                        {
                            id_paad = paad != null ? paad.id_paad : 0,
                            estado = estado != null ? estado.estado : (from e in db.Estados where e.id_estado == 1 select e.estado).FirstOrDefault(),
                            estado_valor = estado != null ? estado.id_estado : 1,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre,
                        }).ToList();
            }
            return list;
        }
        public List<RegistroPAAD> GetRecordPAADs(int period = 0, int career = 0)
        {
            List<RegistroPAAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from paad in db.PAADs
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        where estado.id_estado == 3
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        where period > 0 ? periodo.id_periodo == period : true
                        join carrera in db.Carreras
                        on paad.carrera equals carrera.id_carrera
                        where career > 0 ? carrera.id_carrera == career : true
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        select new RegistroPAAD
                        {
                            id_paad = paad.id_paad,
                            estado = estado.estado,
                            estado_valor = estado.id_estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre
                        }).ToList();
            }
            return list;
        }
        public List<SelectListItem> GetStates()
        {
            List<SelectListItem> periods = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                periods = (from estado in db.Estados
                           select new SelectListItem
                           {
                               Text = estado.estado,
                               Value = estado.id_estado.ToString()
                           }).ToList();
                periods.Insert(0, new SelectListItem { Text = "Todos", Value = "0" });
            }
            return periods;
        }
        public List<SelectListItem> GetCareers()
        {
            List<SelectListItem> periods = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                periods = (from carrera in db.Carreras
                           select new SelectListItem
                           {
                               Text = carrera.carrera,
                               Value = carrera.id_carrera.ToString()
                           }).ToList();
                periods.Insert(0, new SelectListItem { Text = "Todos", Value = "0" });
            }
            return periods;
        }
        public List<SelectListItem> GetPeriods()
        {
            List<SelectListItem> periods = null;
            using (var db = new DB_PAAD_IADEntities())
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
            using (var db = new DB_PAAD_IADEntities())
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
        public ActionResult ViewIAAD(int id)
        {
            InfoPAADCLS info = GetInfoIAAD(id);
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_iad);
            ViewBag.activities = GetActivitiesIAD(info.id_iad);
            ViewBag.msg = GetMessages(info.id_iad);
            return View("ViewIAAD_Coordinador");
        }
        // ListActiveIAADs Actions
        public ActionResult ListActiveIAADs()
        {
            ViewBag.list = GetActiveIAADs();
            ViewBag.states = GetStates();
            ViewBag.careers = GetCareers();
            return View("ListActiveIAADs_Subdirector");
        }

        public List<RegistroIAD> GetActiveIAADs(int state = 0)
        {
            Docentes doc = (Docentes)Session["user"];
            List<RegistroIAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from docente in db.Docentes
                        join paad in db.IADs
                        on docente.id_docente equals paad.docente into gpaad
                        from paad in gpaad.DefaultIfEmpty()
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado into gestado
                        from estado in gestado.DefaultIfEmpty()
                        where state > 0 && !(estado == null && state == 1) ? estado.id_estado == state && estado.id_estado != 3 : estado.id_estado != 3
                        join carrera in db.Carreras
                        on docente.carrera equals carrera.id_carrera
                        where carrera.id_carrera == doc.carrera
                        from periodo in db.Periodos
                        where periodo.activo == true
                        select new RegistroIAD
                        {
                            id_iad = paad != null ? paad.id_iad : 0,
                            // estado = int.Parse(estado != null ? estado.estado : (from e in db.Estados where e.id_estado == 1 select e.estado).FirstOrDefault()),
                            //estado = estado != null ? estado.id_estado : 1,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            // numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre,
                        }).ToList();
            }
            return list;
        }

        public List<RegistroIAD> GetRecordIAADs(int period = 0)
        {
            Docentes doc = (Docentes)Session["user"];
            List<RegistroIAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from paad in db.IADs
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        where estado.id_estado == 3
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        where period > 0 ? periodo.id_periodo == period : true
                        join carrera in db.Carreras
                        on paad.carrera equals carrera.id_carrera
                        where carrera.id_carrera == doc.carrera
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        select new RegistroIAD
                        {
                            id_iad = paad.id_iad,
                            estado = estado.estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,

                            nombre_docente = docente.nombre
                        }).ToList();
            }
            return list;
        }


        public ActionResult FilterRecordIAADs(string filter_period)
        {
            List<RegistroIAD> list = GetRecordIAADs(Convert.ToInt32(filter_period));
            return PartialView("_ListIAADs", list);
        }
        public InfoPAADCLS GetInfoIAAD(int id)
        {
            InfoPAADCLS info = new InfoPAADCLS();
            Docentes doc = (Docentes)Session["user"];
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from paad in db.IADs
                        where paad.id_iad == id
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        select new InfoPAADCLS
                        {
                            id_paad = paad.id_iad,
                            status_value = paad.estado,
                            status_name = estado.estado,
                            active = periodo.activo
                        }).FirstOrDefault();
            }
            return info;
        }


        public List<ActivityCLS> GetActivitiesIAD(int id)
        {
            List<ActivityCLS> activities = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                activities = (from activity in db.Actividades
                              where activity.id_iad == id
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
                                  id_iad = activity.id_paad
                              }).ToList();
            }
            return activities;
        }
        //ListRecordIAADs Actions
        public ActionResult ListRecordIAADs()
        {
            ViewBag.list = GetRecordIAADs();
            ViewBag.period = GetPeriods();
            ViewBag.careers = GetCareers();
            return View("ListRecordIAADs_Subdirector");
        }

    }
}