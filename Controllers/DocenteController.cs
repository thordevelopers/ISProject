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
using ISProject.Filters;

namespace ISProject.Controllers
{
    //Esta etiqueta liga todas las acciones de la clase al filtro "FilterDocente", lo que significa que antes de la ejecucion del cualquier accion en la clase primero se va ejectuar las
    //acciones del filtro.
    [FilterDocente]
    public class DocenteController : Controller
    {
        /*Se inicializa un auxiliar para las funciones de aunteticacion, mas detalles sobre estas funciones las puedes encontrar en el controlador "AuthenticationController" */
        UtilitiesController util = new UtilitiesController();
        //Vista de inicio para el docente
        public ActionResult Home()
        {
            util.IsClose();
            return View("HomeDocente");
        }
        //Acciones de la vista ------------------------------------------------ ModifyPAAD ------------------------------------------------
        public ActionResult ModifyPAAD()
        {
            //Se obtiene la info basica del paad
            InfoPeriodCLS info_period = util.GetInfoPeriod();
            if (info_period.is_close)
                return View("NotActivePeriod_Docente"); //No hay periodo activo
            InfoPAADCLS info = GetInfoPAAD();
            if (info_period.is_close_paad)
            {
                if(info==null || info.status_value<3)
                    return View("NotActivePeriod_Docente"); //No se lleno el formato paad o no se aprobo y ya no se puede hacer nada
                else
                    return RedirectToAction("ViewPAAD", new { id = info.id_paad }); //Se lleno el paad correctamente y esta aprobado
            }
            if (!info_period.on_time_paad)
            {
                if (info==null)
                    return View("NotActivePeriod_Docente"); //Ya no es periodo de entrega y no creo ningun paad
                else if (!info.is_extemporaneous)
                    return RedirectToAction("ViewPAAD", new { id = info.id_paad }); //Se creo un paad pero ya no es periodo de entrega y no es extemporaneo
            }
            // Se valida si el paad no esta en edicion que lo redirija vista de visualizacion.
            if (info != null && info.status_value != 1)
                return RedirectToAction("ViewPAAD", new { id = info.id_paad });
            //Se obtienen los datos necesarios y se asignan a la ViewBag de la vista, para ser extraidos ahi.
            ViewBag.info = info;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            //El 1 indica que el mensaje que se esta buscando es de tipo: rechazo del paad
            ViewBag.msg = GetMessages(info.id_paad,1);
            return View();
        }
        /* Accion que se dispara cuando se selecciona la opcion de crear o editar una accion del paad
         * Recive el id del paad asi como el id de la actividad
         * Regresa una vista partial*/
        public ActionResult ModalActivity(int mdl_id_paad, int mdl_id_activity)
        {
            ActivityCLS modal = null;
            //Verifica si el id de la actividad es mayor a cero, si lo es busca esa actividad en la base de datos
            //si la actividad no es mayor a cero significa que se quiere crear una nueva actividad por lo que se crea el objeto y se regresa.
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
            //Aqui se regresa la vista pertial, especificando el nombre de la vista asi como el modelo que usara para rellenarse
            return PartialView("_ModalActivityPAAD",modal);
        }
        //esta etiqueta especifica que solo recibira peticiones de tipo post
        [HttpPost]
        /*Esta accion es llamada cuando se da el boton de guardar actividad del modal 
          Recibe el modelo que contiene toda la info de la actividad
          Devuelve Json con un estado de respuesta, un mensaje de respuesta y opcionalmente una vista parcial. */
        public ActionResult SaveActivity(ActivityCLS model)
        {
            //Verifica que las validaciones puestas en el modelo se cumplan si no regresa Json junto con una vista parcial 
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Status = 2,
                    Message = "Invalid",
                    //La funcion que manda llamar transforma una vista partial a un string html entendible para java script
                    AjaxResponse = RenderRazorViewToString("_ModalActivityPAAD", model)
                });
            }
            else
            {
                //Aqui dependiendo del id se crea una nueva actividad o se guardan los cambios de una actividad existente
                if (model.id != 0)
                {
                    using (var db = new DB_PAAD_IADEntities())
                    {
                        //Se busca la activadad por el id para modificarla y guardar los cambios
                        Actividades act_db = db.Actividades.Single(p => p.id_actividad == model.id);
                        act_db.actividad = model.actividad;
                        act_db.actividad = model.actividad;
                        act_db.produccion = model.produccion;
                        act_db.lugar = model.lugar;
                        act_db.porcentaje_inicial = model.porcentaje_inicial;
                        act_db.cacei = model.cacei;
                        act_db.cuerpo_academico = model.cuerpo_academico;
                        act_db.iso = model.iso;
                        //aqui se guardan los cambios a la base de datos.
                        db.SaveChanges();
                    }
                }
                else
                {
                    using (var db = new DB_PAAD_IADEntities())
                    {
                        //Aqui se crea un nuevo registro en la base de datos especificando la tabla y enviendo un medelo de esa clase con la info de la nueva actividad.
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
                        //aqui se guardan los cambios a la base de datos.
                        db.SaveChanges();
                    }
                }
                //Se llama a la funcion de obtener las activdades del paad debido a que esta vez no se actualizara el modal sino la tabla de actividades
                List<ActivityCLS> list = GetActivities(model.id_paad);
                return Json(new
                {
                    Status = 1,
                    Message = "Success",
                    AjaxResponse = RenderRazorViewToString("_EditActivitiesTable", list)
                });
            }
        }
        /*Esta funcion sirve para eliminar una actividad 
         Recibe el id del paad y el id de la actividad a borrar
         Regresa una vista parcial de la tabla de actividades*/

        public ActionResult DeleteActivity(int del_id_paad, int del_id_activity)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                Actividades act_db = db.Actividades.Single(p => p.id_actividad == del_id_activity);
                //Esta es la instruccion para eliminar un elemento de la base de datos, especificas la tabla y le pasa un modelo de la tabla con la llave primaria
                db.Actividades.Remove(act_db);
                db.SaveChanges();
            }
            List<ActivityCLS> list = GetActivities(del_id_paad);
            return PartialView("_EditActivitiesTable", list);
        }
        /*Esta funcion elimina todas las actividades ligadas a un paad
         Recibe el id del paad
         Regresa una vista parcial de la tabla de actividades*/
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
        /* Esta funcion se llama cuando el docente cambia su cargo a cualquiera de las opciones que hay en la tabla de la base de datos
         * Recibe el id del cargo y el id del paad
         * No devuelve nada*/
        public void ChangeCargo(string id_cargo,int id_paad)
        {
            using(var db = new DB_PAAD_IADEntities())
            {
                PAADs doc_paad = (from paad in db.PAADs where paad.id_paad == id_paad select paad).FirstOrDefault();
                doc_paad.cargo = Convert.ToInt32(id_cargo);
                db.SaveChanges();
            }
        }
        //Acciones de la vista ------------------------------------------------ ViewPAAD ------------------------------------------------
        /* Esta funcion devuelve la vista de ver paad*/
        public ActionResult ViewPAAD(int id)
        {
            InfoPeriodCLS info_period = util.GetInfoPeriod();
            InfoPAADCLS info = GetInfoPAAD(id);
            if (info == null)
                return View("NotFoundError_Docente");
            if (info_period.on_time_paad || (!info_period.is_close_paad && info.is_extemporaneous)) 
            {
                if (info.status_value == 1 && info.active)
                    return RedirectToAction("ModifyPAAD");
            }
            // Verifica si el paad activo esta en edicion, si lo esta redirije la vista a vista ModifyPaad
            
            ViewBag.info = info;
            ViewBag.info_period=info_period;
            ViewBag.header = GetHeader(info.id_paad);
            ViewBag.activities = GetActivities(info.id_paad);
            ViewBag.msg = GetMessages(info.id_paad,3);
            return View("ViewPAAD_Docente");
        }
        /* Esta accion aplica las acciones sobre el paad como entrgar, cancelar entrega, solicitar modificacion y cancelar solicitud
         * Recibe las credenciales a autenticar, el id del padd, la accion a realizar y el mensaje para la solicitud de manera opcional
         * Regresa un json con el estado de la respuesta, el mensaje de respuesta, y una vista parcial en string */
        public ActionResult ApplyActionPAAD(AuthenticationCLS credentials,int id_paad,int action_paad,string message_modif=null)
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
            Docentes doc = ((Docentes)Session["user"]);
            //Valida que la autenticacion sea correcta y que el correo de la autenticacion se el mismo que el de la sesion
            if (!util.AuthenticateCredentials(credentials.email, credentials.password) || doc.correo != credentials.email)
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
                Mensajes mssg=null;
                
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
                    //Acciones para el caso de Entregar PAAD
                    paad.estado = 2;
                    paad.extemporaneo = false;
                    mssg = db.Mensajes.Where(p => p.paad == id_paad && p.tipo == 1).FirstOrDefault();
                    paad.firma_docente = Guid.NewGuid().ToString("N");
                }
                else if (action_paad == 2)
                {
                    //Acciones para el caso de Cancelar Entrega de PAAD
                    paad.estado = 1;
                    paad.firma_docente = null;
                }
                else if (action_paad == 3)
                {
                    //Acciones para el caso de Solicitar Modificacion de PAAD
                    paad.estado = 4;
                    mssg = db.Mensajes.Where(p => p.paad == id_paad && p.tipo == 3).FirstOrDefault();
                    db.Mensajes.Add(new Mensajes
                    {
                        paad = paad.id_paad,
                        tipo = 2,
                        mensaje=message_modif
                    });
                }
                else if (action_paad == 4)
                {
                    //Acciones para el caso de Cancelar Solicitud de Modificacion de PAAD
                    paad.estado = 3;
                    mssg = db.Mensajes.Where(p => p.paad == id_paad && p.tipo == 2).FirstOrDefault();
                }
                //Si hay mensajes que borrar los elimina, por lo generar por cada paad solo hay un mensaje activo que mostrar, en caso de eliminar varios al mismo tiempo se debera modificar.
                if (mssg!=null)
                    db.Mensajes.Remove(mssg);
                db.SaveChanges();
            }
            return Json(new
            {
                Status = 1,
                Message = "Success"
            });
        }
        //Acciones de la vista ------------------------------------------------ ListRecordPAADs ------------------------------------------------
        /* Esta accion carga la informacion para la vista*/
        public ActionResult ListRecordPAADs()
        {
            ViewBag.list = GetRecordPAADs();
            ViewBag.periods = GetPeriods();
            return View("ListRecordPAADs_Docente");
        }
        /* Esta accion se dispara cuando se el valor seleccionado del dropdownlist cambia
         * Filtra la lista segun el valor seleccionado del dropdownlist
         * Recibe el id del periodo por el cual se filtrara 
         * Regresa una vista parcial de la tabla de paads */
        public ActionResult FilterRecordPAADs(string id_period)
        {
            List<RegistroPAAD> list = GetRecordPAADs(Convert.ToInt32(id_period));
            return PartialView("_ListPAADs", list);
        }
        //Funciones de  ------------------------------------------------ Utilidades ------------------------------------------------
        /* Esta funcion llena el modelo de InfoPAADCLS con la informacion de la base de datos 
         * Recibe de manera opcional un id del paad 
         * Regresa el modelo lleno*/
        public InfoPAADCLS GetInfoPAAD(int id=0)
        {
            InfoPAADCLS info = new InfoPAADCLS();
            Docentes doc = (Docentes)Session["user"];
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from paad in db.PAADs
                        where id > 0 ? paad.id_paad == id : paad.docente == doc.id_docente
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        where id > 0 ? true : periodo.activo == true
                        select new InfoPAADCLS
                        {
                            id_paad = paad.id_paad,
                            status_value = paad.estado,
                            status_name = estado.estado,
                            active = periodo.activo,
                            is_extemporaneous = paad.extemporaneo
                        }).FirstOrDefault();
                if (info == null && id == 0)
                {
                    db.PAADs.Add(new PAADs
                    {
                        estado = 1,
                        periodo = (from periodo in db.Periodos where periodo.activo == true select periodo.id_periodo).FirstOrDefault(),
                        carrera = 1,
                        docente = doc.id_docente,
                        categoria_docente = 1,
                        horas_clase = 10,
                        horas_investigacion = 10,
                        horas_gestion = 10,
                        horas_tutorias = 10,
                        cargo = 1, 
                        extemporaneo = false
                    });
                    db.SaveChanges();
                    info = (from paad in db.PAADs
                            where paad.docente == doc.id_docente
                            join estado in db.Estados
                            on paad.estado equals estado.id_estado
                            join periodo in db.Periodos
                            on paad.periodo equals periodo.id_periodo
                            where periodo.activo == true
                            select new InfoPAADCLS
                            {
                                id_paad = paad.id_paad,
                                status_value = paad.estado,
                                status_name = estado.estado,
                                active = periodo.activo,
                                is_extemporaneous = paad.extemporaneo
                            }).FirstOrDefault();
                }
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
                //Obtiene la informacion
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
                              horas_tutorias = paads.horas_tutorias,
                              id_paad = id
                          }).First();
                //Obtiene la lista de cargos
                header.cargos = (from cargo in db.Cargos
                                 select new SelectListItem
                                 {
                                     Text = cargo.cargo,
                                     Value = cargo.id_cargo.ToString()
                                 }).ToList();
                //Obtiene el id del cargo que tiene seleccionado
                int id_cargo = (from paad in db.PAADs where paad.id_paad == id select paad.cargo).FirstOrDefault();
                //Marca como seleccionado el cargo en la lista de cargos.
                header.cargos.Where(p => p.Value == id_cargo.ToString()).FirstOrDefault().Selected = true; 
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
        /* Esta accion recupera todos los paads del docente de la base de datos 
         * Recibe de forma opcional el id del periodo por el cual filtrar si no es enviado regresa todos sin importar el periodo
         * Regresa una lista con los modelos de los paad*/
        public List<RegistroPAAD> GetRecordPAADs(int period=0)
        {
            bool onTime = util.IsOnTimePAAD();
            List<RegistroPAAD> list = null;
            using (var db = new DB_PAAD_IADEntities())
            {
                Docentes doc = ((Docentes)Session["user"]);
                list = (from paad in db.PAADs
                        where paad.docente == doc.id_docente && paad.estado==3
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        where period > 0 ? periodo.id_periodo == period : true
                        join carrera in db.Carreras
                        on paad.carrera equals carrera.id_carrera
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        select new RegistroPAAD
                        {
                            id_paad = paad.id_paad,
                            estado = estado.estado,
                            estado_valor = 0,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre
                        }).ToList();
            }
            return list;
        }
        /* Esta accion recupera los periodos 
         * No recibe argumentos
         * Regresa una lista con los modelos de los periodos*/
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
        /* Esta accion recupera los mensajes de un paad la base de datos 
         * Recibe el id del paad 
         * Regresa un modelo con la informacion del mensaje*/
        public MessageCLS GetMessages(int id,int tipo)
        {
            MessageCLS msg;
            using(var db=new DB_PAAD_IADEntities())
            {
                msg = (from message in db.Mensajes
                       where message.paad == id && message.tipo == tipo
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