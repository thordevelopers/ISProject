﻿using ISProject.Filters;
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
    [FilterSubdirector]
    public class SubdirectorController : Controller
    {
        /*Se inicializa un auxiliar para las funciones de aunteticacion, mas detalles sobre estas funciones las puedes encontrar en el controlador "AuthenticationController" */
        UtilitiesController util = new  UtilitiesController();
        #region Home Actions
        //Acciones de la vista ------------------------------------------------ HomeSubdirector ------------------------------------------------
        //Vista de inicio para el subdirector
        public ActionResult Home()
        {
            return View("HomeSubdirector");
        }
        #endregion
        #region PAAD Actions
        //Acciones de la vista ------------------------------------------------ ViewPAAD ------------------------------------------------
        /* Esta accion corresponde a la vista ViewPAAD
         * Recibe el id del paad 
         * Devuelve la vista*/
        public ActionResult ViewPAAD(int id)
        {
            util.IsClose();
            //Valida que el id del paad se valido si no redirecciona a home
            if (id < 1)
                return RedirectToAction("Home");
            InfoPAADCLS info = GetInfoPAAD(id);
            if (info == null)
                return RedirectToAction("Home");
            //~~~~~~~Poner redirecion a error not found
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            //Valida si el paad a ver es del director para mostrar o no los mensajes de rechazo o aprobacion
            if (info.isdirector)
                ViewBag.msg = GetMessages(info.id_paad);
            else
                ViewBag.msg = null;
            return View("ViewPAAD_Subdirector");
        }
        /* Esta accion aplica las acciones sobre el paad como rechazar paad, aprobar paad, aprobar modificacion y rechazar modificacion
         * Recibe las credenciales a autenticar, el id del padd, la accion a realizar y el mensaje de rechazo de manera opcional
         * devuelve un json con el estado de la respuesta, el mensaje de respuesta, y una vista parcial en string */
        public ActionResult ApplyActionPAAD(AuthenticationCLS credentials, int id_paad, int action_paad, string reject_message)
        {
            //Valida los campos del modelo de las credenciales
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            //Obtiene los datos de la sesion del usuario
            Administrativos doc = ((Administrativos)Session["administ"]);
            //Valida que la autenticacion sea correcta, que el correo de la autenticacion se el mismo que el de la sesion y que la cuenta tenga el nivel de permisos necesarios
            if (!util.AuthenticateCredentials(credentials.email, credentials.password) || doc.rol < 2 || doc.correo != credentials.email)
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
                Mensajes mssg = null;
                PAADs paad = db.PAADs.Where(p => p.id_paad == id_paad).FirstOrDefault();
                bool isNotDirectorOrNull = paad != null ? !util.IsDirector(paad.docente) : true;
                //Valida que el paad sea del director
                if (isNotDirectorOrNull)
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
                    //Acciones para el caso de Rechazar PAAD
                    paad.estado = 1;
                    paad.firma_docente = null;
                    db.Mensajes.Add(new Mensajes
                    {
                        paad = paad.id_paad,
                        tipo = 1,
                        mensaje = reject_message
                    });
                }
                else if (action_paad == 2)
                {
                    //Acciones para el caso de Aprobar PAAD
                    paad.estado = 3;
                    paad.firma_director = Guid.NewGuid().ToString("N");
                }
                else if (action_paad == 3)
                {
                    //Acciones para el caso de Rechazar Solicitud
                    paad.estado = 3;
                    db.Mensajes.Add(new Mensajes
                    {
                        paad = paad.id_paad,
                        tipo = 3,
                        mensaje = reject_message
                    });
                    mssg = db.Mensajes.Where(p => p.paad == id_paad && p.tipo == 2).FirstOrDefault();
                }
                else if (action_paad == 4)
                {
                    //Acciones para el caso de Aprobar Solicitud
                    paad.estado = 1;
                    paad.firma_docente = null;
                    paad.firma_director = null;
                    mssg = db.Mensajes.Where(p => p.paad == id_paad && p.tipo == 2).FirstOrDefault();
                }
                //Si hay mensajes que borrar los elimina, por lo generar por cada paad solo hay un mensaje activo que mostrar, en caso de eliminar varios al mismo tiempo se debera modificar.
                if (mssg != null)
                    db.Mensajes.Remove(mssg);
                db.SaveChanges();
            }
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        //Acciones de la vista ------------------------------------------------ ListActivePAADs ------------------------------------------------
        /* Esta accion muestra la vista de ListActivePAADs*/
        public ActionResult ListActivePAADs()
        {
            InfoPeriodCLS info_period = util.GetInfoPeriod();
            if (info_period.is_close || info_period.is_close_paad)
                return View("HomeSubdirector"); //"ErrorMessage_Subdirector", new ErrorMessageCLS { message = "No hay periodo activo" });
            ViewBag.list = GetActivePAADs();
            ViewBag.states = GetStates();
            ViewBag.careers = GetCareers();
            return View("ListActivePAADs_Subdirector");
        }
        /* Esta accion se dispara cuando el valor seleccionado de cualquier dropdownlist cambia
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del estado, el id de la carrera
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterActivePAADs(string filter_state, string filter_career)
        {
            List<RegistroPAAD> list = GetActivePAADs(Convert.ToInt32(filter_state), Convert.ToInt32(filter_career));
            return PartialView("_ListPAADs", list);
        }
        //Acciones de la vista ------------------------------------------------ ListRecordPAADs ------------------------------------------------
        /* Esta accion muestra la vista de ListRecordPAADs*/
        public ActionResult ListRecordPAADs()
        {
            ViewBag.list = GetRecordPAADs();
            ViewBag.period = GetPeriods();
            ViewBag.careers = GetCareers();
            return View("ListRecordPAADs_Subdirector");
        }
        /* Esta accion se dispara cuando se el valor seleccionado de cualquier dropdownlist cambie
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del periodo, el id de la carrera 
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterRecordPAADs(string filter_period, string filter_career)
        {
            List<RegistroPAAD> list = GetRecordPAADs(Convert.ToInt32(filter_period), Convert.ToInt32(filter_career));
            return PartialView("_ListPAADs", list);
        }
        #endregion
        #region IAD Actions
        //Acciones de la vista ------------------------------------------------ ViewIAD ------------------------------------------------
        /* Esta accion corresponde a la vista ViewIAD
         * Recibe el id del iad 
         * Devuelve la vista*/
        public ActionResult ViewIAD(int id)
        {
            util.IsClose();
            //Valida que el id del paad se valido si no redirecciona a home
            if (id < 1)
                return RedirectToAction("Home");
            InfoIADCLS info = GetInfoIAD(id);
            ViewBag.info = info;
            ViewBag.header = GetHeaderIAD(info.id_iad);
            ViewBag.activities = GetActivitiesIAD(info.id_iad);
            //Valida si el paad a ver es del director para mostrar o no los mensajes de rechazo o aprobacion
            if (info.isdirector)
                ViewBag.msg = GetMessagesIAD(info.id_iad);
            else
                ViewBag.msg = null;
            return View("ViewIAD_Subdirector");
        }
        /* Esta accion aplica las acciones sobre el paad como rechazar paad, aprobar paad, aprobar modificacion y rechazar modificacion
         * Recibe las credenciales a autenticar, el id del padd, la accion a realizar y el mensaje de rechazo de manera opcional
         * devuelve un json con el estado de la respuesta, el mensaje de respuesta, y una vista parcial en string */
        public ActionResult ApplyActionIAD(AuthenticationCLS credentials, int id_iad, int action_iad, string reject_message)
        {
            //Valida los campos del modelo de las credenciales
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            //Obtiene los datos de la sesion del usuario
            Administrativos doc = ((Administrativos)Session["administ"]);
            //Valida que la autenticacion sea correcta, que el correo de la autenticacion se el mismo que el de la sesion y que la cuenta tenga el nivel de permisos necesarios
            if (!util.AuthenticateCredentials(credentials.email, credentials.password) || doc.rol != 2 || doc.correo != credentials.email)
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
                Mensajes mssg = null;

                IADs iad = db.IADs.Where(p => p.id_iad == id_iad).FirstOrDefault();
                bool isNotDirectorOrNull = iad != null ? !util.IsDirector(iad.docente) : true;
                //Valida que el paad sea del director
                if (isNotDirectorOrNull)
                {
                    credentials.message = "Correo y/o contraseña incorrectos";
                    return Json(new
                    {
                        Status = 3,
                        Message = "Error",
                        AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                    });
                }
                else if (action_iad == 1)
                {
                    //Acciones para el caso de Rechazar PAAD
                    iad.estado = 1;
                    iad.firma_docente = null;
                    db.Mensajes.Add(new Mensajes
                    {
                        iad = iad.id_iad,
                        tipo = 1,
                        mensaje = reject_message
                    });
                }
                else if (action_iad == 2)
                {
                    //Acciones para el caso de Aprobar PAAD
                    iad.estado = 3;
                    iad.firma_director = Guid.NewGuid().ToString("N");
                }
                else if (action_iad == 3)
                {
                    //Acciones para el caso de Rechazar Solicitud
                    iad.estado = 3;
                    db.Mensajes.Add(new Mensajes
                    {
                        iad = iad.id_iad,
                        tipo = 3,
                        mensaje = reject_message
                    });
                    mssg = db.Mensajes.Single(p => p.iad == id_iad && p.tipo == 2);
                }
                else if (action_iad == 4)
                {
                    //Acciones para el caso de Aprobar Solicitud
                    iad.estado = 1;
                    iad.firma_docente = null;
                    iad.firma_director = null;
                    mssg = db.Mensajes.Where(p => p.iad == id_iad && p.tipo == 2).FirstOrDefault();
                }
                //Si hay mensajes que borrar los elimina, por lo generar por cada paad solo hay un mensaje activo que mostrar, en caso de eliminar varios al mismo tiempo se debera modificar.
                if (mssg != null)
                    db.Mensajes.Remove(mssg);
                db.SaveChanges();
            }
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        //Acciones de la vista ------------------------------------------------ ListActiveIADs ------------------------------------------------
        /* Esta accion muestra la vista de ListActiveIADs*/
        public ActionResult ListActiveIADs()
        {
            InfoPeriodCLS info_period = util.GetInfoPeriod();
            if (info_period.is_close || !info_period.is_close_paad)
                return View("HomeSubdirector"); //"ErrorMessage_Subdirector", new ErrorMessageCLS { message = "No hay periodo activo" });
            ViewBag.list = GetActiveIADs();
            ViewBag.states = GetStates();
            ViewBag.careers = GetCareers();
            return View("ListActiveIADs_Subdirector");
        }
        /* Esta accion se dispara cuando el valor seleccionado de cualquier dropdownlist cambia
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del estado, el id de la carrera
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterActiveIADs(string filter_state, string filter_career)
        {
            List<RegistroIAD> list = GetActiveIADs(Convert.ToInt32(filter_state), Convert.ToInt32(filter_career));
            return PartialView("_ListIADs", list);
        }
        //Acciones de la vista ------------------------------------------------ ListRecordIADs ------------------------------------------------
        /* Esta accion muestra la vista de ListRecordIADs*/
        public ActionResult ListRecordIADs()
        {
            ViewBag.list = GetRecordIADs();
            ViewBag.period = GetPeriods();
            ViewBag.careers = GetCareers();
            return View("ListRecordIADs_Subdirector");
        }
        /* Esta accion se dispara cuando se el valor seleccionado de cualquier dropdownlist cambie
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del periodo, el id de la carrera 
         * Regresa una vista parcial de la tabla con los paads filtrados */
        public ActionResult FilterRecordIADs(string filter_period, string filter_career)
        {
            List<RegistroIAD> list = GetRecordIADs(Convert.ToInt32(filter_period), Convert.ToInt32(filter_career));
            return PartialView("_ListIADs", list);
        }
        #endregion
        #region ChangeSubdirector Actions
        //Acciones de la vista ------------------------------------------------ ChangeSubdirectorAccount ------------------------------------------------
        /* Esta accion muestra la vista de ChangeDirectorAccount*/
        public ActionResult ChangeSubdirectorAccount()
        {
            ViewBag.director = GetSubdirector();
            ViewBag.accounts = GetAccounts();
            return View("ChangeSubdirectorAccount");
        }
        /* Esta accion se llama cuando le da al boton de aceptar en el modal de credenciales
         * Esta funcion sirve para cambiar el campo isdirector de la tabla de docentes de una cuenta a otra
         * Recibe las credenciales y el id del docente que sera el nuevo director
         * Regresa un json con el estado de la respuesta, el mensaje de respuesta, y una vista parcial en string */
        public ActionResult ChangeSubdirector(AuthenticationCLS credentials, int id_docente)
        {
            //Valida los campos del modelo de las credenciales
            if (!ModelState.IsValid)
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                });
            //Obtiene los datos de la sesion del usuario
            Administrativos doc = ((Administrativos)Session["administ"]);
            //Valida que la autenticacion sea correcto, que el correo de la autenticacion se el mismo que el de la sesion y que se tengan los permisos necesario para realizar esa accion
            if (!util.AuthenticateCredentials(credentials.email, credentials.password) || doc.rol != 2 || doc.correo != credentials.email)
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
                Docentes docente = db.Docentes.Where(p => p.id_docente == id_docente).FirstOrDefault();
                bool isDirectorOrNull = docente != null ? util.IsDirector(docente.id_docente) : true;
                //Si el docente no se encutra o ya es director regresa un mensaje de error, si no lo vuelve director
                if (isDirectorOrNull)
                {
                    credentials.message = "Cuenta no encontrada o esta cuenta ya es director";
                    return Json(new
                    {
                        Status = 3,
                        Message = "Error",
                        AjaxResponse = RenderRazorViewToString("_AuthenticateCredentials", credentials)
                    });
                }
                Administrativos director = db.Administrativos.Where(p => p.rol == 2).FirstOrDefault();
                if (director != null)
                    director.docente = docente.id_docente;
                db.SaveChanges();
            }
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        #endregion
        #region Utility Actions
        //Funciones de  ------------------------------------------------ Utilidades ------------------------------------------------
        /* Esta funcion llena el modelo de InfoPAADCLS con la informacion de la base de datos 
         * Recibe el id del paad 
         * Regresa el modelo lleno*/
        public InfoPAADCLS GetInfoPAAD(int id)
        {
            InfoPAADCLS info = new InfoPAADCLS();
            Administrativos doc = (Administrativos)Session["administ"];
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from admin in db.Administrativos
                        where admin.rol == 3
                        from paad in db.PAADs
                        where paad.id_paad == id && paad.estado != 1
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
                            isdirector = paad.docente == admin.docente? true :false
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
            Administrativos doc = (Administrativos)Session["administ"];
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from admin in db.Administrativos
                        where admin.rol == 3
                        from iad in db.IADs
                        where iad.id_iad == id && iad.estado != 1
                        join estado in db.Estados
                        on iad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on iad.periodo equals periodo.id_periodo
                        join docente in db.Docentes
                        on iad.docente equals docente.id_docente
                        select new InfoIADCLS
                        {
                            id_iad = iad.id_iad,
                            status_value = iad.estado,
                            status_name = estado.estado,
                            active = periodo.activo,
                            isdirector = iad.docente == admin.docente ? true : false
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
        /* Obtiene la informacion del encabezado del PAAD de la base de datos
         * Recibe el id del paad
         * Regresa el modelo lleno */
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

                          }).FirstOrDefault();

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
        /* Esta accion recupera todos los paads aprobados de la base de datos 
         * Recibe de forma opcional el id del estado y el id de la carrera, si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroPAAD> GetActivePAADs(int state = 0, int career = 0)
        {
            List<RegistroPAAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from docente in db.Docentes
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
        /* Esta accion recupera todos los paads activos de la base de datos 
         * Recibe de forma opcional el id del estado y el id de la carrera, si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroIAD> GetActiveIADs(int state = 0, int career = 0)
        {
            List<RegistroIAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                list = (from periodo in db.Periodos
                        where periodo.activo == true
                        from docente in db.Docentes
                        join iad in db.IADs
                        on new { id = docente.id_docente, activo = periodo.id_periodo } equals new { id = iad.docente, activo = iad.periodo } into gpaad
                        from iad in gpaad.DefaultIfEmpty()
                        join estado in db.Estados
                        on iad.estado equals estado.id_estado into gestado
                        from estado in gestado.DefaultIfEmpty()
                        where state > 0 && !(estado == null && state == 1) ? estado.id_estado == state && estado.id_estado != 3 : estado.id_estado != 3
                        join carrera in db.Carreras
                        on docente.carrera equals carrera.id_carrera
                        where career > 0 ? carrera.id_carrera == career : true
                        select new RegistroIAD
                        {
                            id_iad = iad != null ? iad.id_iad : 0,
                            extemporaneous = iad != null ? iad.extemporaneo : false,
                            estado = estado != null ? estado.estado : (from e in db.Estados where e.id_estado == 1 select e.estado).FirstOrDefault(),
                            estado_valor = estado != null ? estado.id_estado : 1,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            id_docente = docente.id_docente,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre,
                        }).ToList();
            }
            return list;
        }
        /* Esta accion recupera todos los paads activos de la base de datos 
         * Recibe de forma opcional el id del periodo y el id de la carrera, si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
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
        /* Esta accion recupera todos los paads arpobados de la base de datos 
         * Recibe de forma opcional el id del periodo y el id de la carrera, si vienen vacios se omiten en el filtrado
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroIAD> GetRecordIADs(int period = 0, int career = 0)
        {
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
                        where career > 0 ? carrera.id_carrera == career : true
                        join docente in db.Docentes
                        on iad.docente equals docente.id_docente
                        select new RegistroIAD
                        {
                            id_iad = iad.id_iad,
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
        /* Esta accion recupera los estados 
         * No recibe argumentos
         * Regresa una lista con los modelos de los estados*/
        public List<SelectListItem> GetStates()
        {
            List<SelectListItem> periods = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                periods = (from estado in db.Estados
                           where estado.id_estado != 3
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
        /* Esta accion recupera los mensajes de un paad la base de datos 
         * Recibe el id del paad 
         * Regresa un modelo con la informacion del mensaje*/
        public MessageCLS GetMessages(int id)
        {
            MessageCLS msg;
            using (var db = new DB_PAAD_IADEntities())
            {
                msg = (from message in db.Mensajes
                       where message.paad == id && message.tipo == 2
                       join type in db.TiposDeMensaje
                       on message.tipo equals type.id_tipo_mensaje
                       select new MessageCLS
                       {
                           id_message = message.id_mensaje,
                           paad = message.paad ?? default(int),
                           iad = message.iad ?? default(int),
                           tipo = message.tipo,
                           tipo_nombre = type.tipo,
                           mensaje = message.mensaje
                       }).FirstOrDefault();
            }
            return msg;
        }
        /* Esta accion recupera los mensajes de un paad la base de datos 
         * Recibe el id del paad 
         * Regresa un modelo con la informacion del mensaje*/
        public MessageCLS GetMessagesIAD(int id)
        {
            MessageCLS msg;
            using (var db = new DB_PAAD_IADEntities())
            {
                msg = (from message in db.Mensajes
                       where message.iad == id && message.tipo == 2
                       join type in db.TiposDeMensaje
                       on message.tipo equals type.id_tipo_mensaje
                       select new MessageCLS
                       {
                           id_message = message.id_mensaje,
                           paad = message.paad ?? default(int),
                           iad = message.iad ?? default(int),
                           tipo = message.tipo,
                           tipo_nombre = type.tipo,
                           mensaje = message.mensaje
                       }).FirstOrDefault();
            }
            return msg;
        }
        /* Esta accion recupera los docentes
         * No recibe argumentos
         * Regresa una lista con los modelos de los docentes */
        public List<SelectListItem> GetAccounts()
        {
            List<SelectListItem> accounts = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                accounts = (from docente in db.Docentes
                            join admin in db.Administrativos
                            on docente.id_docente equals admin.docente into gadmin
                            from admin in gadmin.DefaultIfEmpty()
                            where admin.rol!=2 && admin.rol!=3
                            join carrera in db.Carreras
                            on docente.carrera equals carrera.id_carrera
                            select new SelectListItem
                            {
                                Text = "No " + docente.numero_empleado + " | " + docente.nombre + " | " + carrera.carrera + " | " + docente.correo,
                                Value = docente.id_docente.ToString()
                            }).ToList();
                accounts.Insert(0, new SelectListItem { Text = "-- Seleccione --", Value = "0" });
            }
            return accounts;
        }
        /* Esta accion recupera al docente que tiene el campo isdirector en true
         * No recibe argumentos
         * Regresa un modelo con la informacion del docente*/
        public Docentes GetSubdirector()
        {
            Docentes director;
            using (var db = new DB_PAAD_IADEntities())
            {
                director = (from admin in db.Administrativos where admin.rol == 2 join docente in db.Docentes on admin.docente equals docente.id_docente select docente).FirstOrDefault();
            }
            return director;
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
        #endregion
    }
}