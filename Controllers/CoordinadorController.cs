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
    //Esta etiqueta liga todas las acciones de la clase al filtro "FilterSubdirector", lo que significa que antes de la ejecucion del cualquier accion en la clase primero se va ejectuar las
    //acciones del filtro.
    [FilterCoordinador]
    public class CoordinadorController : Controller
    {
        //Acciones de la vista ------------------------------------------------ HomeCoordinador ------------------------------------------------
        //Vista de inicio para el coordinador
        public ActionResult Home()
        {
            return View("HomeCoordinador");
        }
        #region PAAD Actions
        //Acciones de la vista ------------------------------------------------ ViewPAAD ------------------------------------------------
        /* Esta accion corresponde a la vista ViewPAAD
         * Recibe el id del paad 
         * Devuelve la vista*/
        public ActionResult ViewPAAD(int id)
        {
            InfoPAADCLS info = GetInfoPAAD(id);
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            return View("ViewPAAD_Coordinador");
        }
        //Acciones de la vista ------------------------------------------------ ListActivePAADs ------------------------------------------------
        /* Esta accion muestra la vista de ListActivePAADs*/
        public ActionResult ListActivePAADs()
        {
            ViewBag.list = GetActivePAADs();
            ViewBag.states = GetStates();
            return View("ListActivePAADs_Coordinador");
        }
        /* Esta accion se dispara cuando el valor seleccionado del dropdownlist cambia
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del estado
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterActivePAADs(string filter_state)
        {
            List<RegistroPAAD> list = GetActivePAADs(Convert.ToInt32(filter_state));
            return PartialView("_ListPAADs", list);
        }
        //Acciones de la vista ------------------------------------------------ ListRecordPAADs ------------------------------------------------
        /* Esta accion muestra la vista de ListRecordPAADs*/
        public ActionResult ListRecordPAADs()
        {
            ViewBag.list = GetRecordPAADs();
            ViewBag.period = GetPeriods();
            ViewBag.careers = GetCareers();
            return View("ListRecordPAADs_Coordinador");
        }
        /* Esta accion se dispara cuando se el valor seleccionado de cualquier dropdownlist cambie
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del periodo, el id de la carrera 
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterRecordPAADs(string filter_period)
        {
            List<RegistroPAAD> list = GetRecordPAADs(Convert.ToInt32(filter_period));
            return PartialView("_ListPAADs", list);
        }
        #endregion
        #region IAD Actions
        //Acciones de la vista ------------------------------------------------ ViewPAAD ------------------------------------------------
        /* Esta accion corresponde a la vista ViewPAAD
         * Recibe el id del paad 
         * Devuelve la vista*/
        public ActionResult ViewIAD(int id)
        {
            InfoIADCLS info = GetInfoIAD(id);
            ViewBag.info = info;
            ViewBag.header = GetHeaderIAD(info.id_iad);
            ViewBag.activities = GetActivitiesIAD(info.id_iad);
            return View("ViewIAD_Coordinador");
        }
        //Acciones de la vista ------------------------------------------------ ListActiveIADs ------------------------------------------------
        /* Esta accion muestra la vista de ListActiveIADs*/
        public ActionResult ListActiveIADs()
        {
            ViewBag.list = GetActiveIADs();
            ViewBag.states = GetStates();
            return View("ListActiveIADs_Coordinador");
        }
        /* Esta accion se dispara cuando el valor seleccionado del dropdownlist cambia
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del estado
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterActiveIADs(string filter_state)
        {
            List<RegistroIAD> list = GetActiveIADs(Convert.ToInt32(filter_state));
            return PartialView("_ListIADs", list);
        }
        //Acciones de la vista ------------------------------------------------ ListRecordIADs ------------------------------------------------
        /* Esta accion muestra la vista de ListRecordIADs*/
        public ActionResult ListRecordIADs()
        {
            ViewBag.list = GetRecordIADs();
            ViewBag.period = GetPeriods();
            ViewBag.careers = GetCareers();
            return View("ListRecordIADs_Coordinador");
        }
        /* Esta accion se dispara cuando se el valor seleccionado de cualquier dropdownlist cambie
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del periodo, el id de la carrera 
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterRecordIADs(string filter_period)
        {
            List<RegistroIAD> list = GetRecordIADs(Convert.ToInt32(filter_period));
            return PartialView("_ListIADs", list);
        }
        #endregion
        //Funciones de  ------------------------------------------------ Utilidades ------------------------------------------------
        /* Esta funcion llena el modelo de InfoPAADCLS con la informacion de la base de datos 
         * Recibe el id del paad 
         * Regresa el modelo lleno*/
        public InfoPAADCLS GetInfoPAAD(int id )
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
        /* Esta funcion llena el modelo de InfoPAADCLS con la informacion de la base de datos 
         * Recibe el id del paad 
         * Regresa el modelo lleno*/
        public InfoIADCLS GetInfoIAD(int id)
        {
            InfoIADCLS info = new InfoIADCLS();
            Docentes doc = (Docentes)Session["user"];
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from iad in db.IADs
                        where iad.id_iad == id
                        join estado in db.Estados
                        on iad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on iad.periodo equals periodo.id_periodo
                        select new InfoIADCLS
                        {
                            id_iad = iad.id_iad,
                            status_value = iad.estado,
                            status_name = estado.estado,
                            active = periodo.activo
                        }).FirstOrDefault();
            }
            return info;
        }
        /* Obtiene la informacion del encabezado del PAAD de la base de datos
         * Recibe el id del paad
         * Regresa el modelo lleno */
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
        public HeaderIADCLS GetHeaderIAD(int id)
        {
            HeaderIADCLS header = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                header = (from iad in db.IADs
                          where iad.id_iad == id
                          join estado in db.Estados
                          on iad.estado equals estado.id_estado
                          join periodo in db.Periodos
                          on iad.periodo equals periodo.id_periodo
                          join carrera in db.Carreras
                          on iad.carrera equals carrera.id_carrera
                          join docente in db.Docentes
                          on iad.docente equals docente.id_docente
                          join categoria in db.Categorias
                          on iad.categoria_docente equals categoria.id_categoria
                          join cargo in db.Cargos
                          on iad.cargo equals cargo.id_cargo
                          select new HeaderIADCLS
                          {
                              periodo = periodo.periodo,
                              nombre = docente.nombre,
                              numero_empleado = docente.numero_empleado,
                              categoria = categoria.categoria,
                              cargo = cargo.cargo,
                              horas_clase = iad.horas_clase,
                              horas_gestion = iad.horas_gestion,
                              horas_investigacion = iad.horas_investigacion,
                              horas_tutorias = iad.horas_tutorias
                          }).First();

            }
            return header;
        }
        /* Esta accion recupera las actividades de un paad de la base de datos 
         * Recibe el id del paad 
         * Regresa una lista con los modelos de la actividades*/
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
                                  porcentaje = activity.porcentaje_inicial,
                                  cacei = activity.cacei,
                                  cuerpo_academico = activity.cuerpo_academico,
                                  iso = activity.iso,
                                  id_paad = activity.id_paad??default(int)
                              }).ToList();
            }
            return activities;
        }
        /* Esta accion recupera las actividades de un paad de la base de datos 
         * Recibe el id del paad 
         * Regresa una lista con los modelos de la actividades*/
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
                                  porcentaje = activity.porcentaje_final,
                                  cacei = activity.cacei,
                                  cuerpo_academico = activity.cuerpo_academico,
                                  iso = activity.iso,
                                  id_paad = activity.id_paad ?? default(int),
                                  id_iad = activity.id_iad ?? default(int)
                              }).ToList();
            }
            return activities;
        }
        /* Esta accion recupera todos los paads activos de la base de datos segun carrera del coordinador
         * Recibe de forma opcional el id del estado si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroPAAD> GetActivePAADs(int state = 0)
        {
            Docentes doc = (Docentes)Session["user"];
            List<RegistroPAAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from docente in db.Docentes
                        where docente.rol == 1
                        join paad in db.PAADs
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
        /* Esta accion recupera todos los paads activos de la base de datos segun carrera del coordinador
         * Recibe de forma opcional el id del estado si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroIAD> GetActiveIADs(int state = 0)
        {
            Docentes doc = (Docentes)Session["user"];
            List<RegistroIAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from docente in db.Docentes
                        where docente.rol==1
                        join iad in db.IADs
                        on docente.id_docente equals iad.docente into gpaad
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
        /* Esta accion recupera todos los paads aprobados de la base de datos segun la carrera del coordinador
         * Recibe de forma opcional el id del periodo y el id de la carrera, si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroPAAD> GetRecordPAADs(int period = 0)
        {
            Docentes doc = (Docentes)Session["user"];
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
                        where carrera.id_carrera == doc.carrera
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        select new RegistroPAAD
                        {
                            id_paad = paad.id_paad,
                            estado = estado.estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre
                        }).ToList();
            }
            return list;
        }
        /* Esta accion recupera todos los paads aprobados de la base de datos segun la carrera del coordinador
         * Recibe de forma opcional el id del periodo y el id de la carrera, si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroIAD> GetRecordIADs(int period = 0)
        {
            Docentes doc = (Docentes)Session["user"];
            List<RegistroIAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from iad in db.IADs
                        join estado in db.Estados
                        on iad.estado equals estado.id_estado
                        where estado.id_estado == 3
                        join periodo in db.Periodos
                        on iad.periodo equals periodo.id_periodo
                        where period > 0 ? periodo.id_periodo == period : true
                        join carrera in db.Carreras
                        on iad.carrera equals carrera.id_carrera
                        where carrera.id_carrera == doc.carrera
                        join docente in db.Docentes
                        on iad.docente equals docente.id_docente
                        select new RegistroIAD
                        {
                            id_iad = iad.id_iad,
                            estado = estado.estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre
                        }).ToList();
            }
            return list;
        }
        /* Esta accion recupera los estados 
         * No recibe argumentos
         * Regresa una lista con los modelos de los estados*/
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
        /* Esta accion recupera las carreras 
         * No recibe argumentos
         * Regresa una lista con los modelos de las carreras*/
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
        /* Esta accion recupera los periodos 
         * No recibe argumentos
         * Regresa una lista con los modelos de los periodos*/
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