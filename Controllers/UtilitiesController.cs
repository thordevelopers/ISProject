using ISProject.Models;
using Rotativa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Controllers
{
    public class UtilitiesController : Controller
    {
        //Funciones de  ------------------------------------------------ Utilidades ------------------------------------------------
        //Esta accion no funciona por que no se cuenta con un certificado ssl valido
        public ActionResult SendEmail()
        {
            MailMessage mailMessage = new MailMessage("test.account.machine@gmail.com", "test.account.machine@gmail.com");
            mailMessage.Subject = "Test message";
            mailMessage.Body = "If you get this message all is okey";

            SmtpClient smtpClient = new SmtpClient("smtp.gmail.com",587);
            smtpClient.Credentials = new System.Net.NetworkCredential()
            {
                UserName = "test.account.machine@gmail.com",
                Password = "KuZ5!2QKvbdUjnzz7pkmfY"
            };
            smtpClient.EnableSsl = true;
            smtpClient.Send(mailMessage);
            return RedirectToAction("Home","Director");
        }
        /* Esta accion es llamada cuando se presiona la opcion de ver paad en cualquier listado paad
         * Esta accion genera un document pdf del paad
         * Recibe el id del paad
         * Regresa una vista como pdf*/
        public ActionResult ViewPDF_PAAD(int id)
        {
            ViewPDFCLS paad = GetPDFInfo_PAAD(id);
            //El view as paad regresa la vista espeficada llenada con el modelo dado como un documento pdf segun los argumentos mandado entre las llaves
            return new ViewAsPdf("ViewPDF",paad)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
            };
        }
        /* Esta accion es llamada cuando se presiona la opcion de ver paad en cualquier listado paad
         * Esta accion genera un document pdf del paad
         * Recibe el id del paad
         * Regresa una vista como pdf*/
        public ActionResult ViewPDF_IAD(int id)
        {
            ViewPDFCLS paad = GetPDFInfo_PAAD(id);
            //El view as paad regresa la vista espeficada llenada con el modelo dado como un documento pdf segun los argumentos mandado entre las llaves
            return new ViewAsPdf("ViewPDF", paad)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
            };
        }
        /* Esta funcion devuelve el modelo para la vista de ViewPDF
         * Recibe el id del paad
         * Regresa el modelo llenado*/
        public ViewPDFCLS GetPDFInfo_PAAD(int id)
        {
            ViewPDFCLS info;
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from paad in db.PAADs
                        where paad.id_paad == id
                        join estado in db.Estados
                        on paad.estado equals estado.id_estado
                        join periodo in db.Periodos
                        on paad.periodo equals periodo.id_periodo
                        join carrera in db.Carreras
                        on paad.carrera equals carrera.id_carrera
                        join docente in db.Docentes
                        on paad.docente equals docente.id_docente
                        join categoria in db.Categorias
                        on paad.categoria_docente equals categoria.id_categoria
                        join cargo in db.Cargos
                        on paad.cargo equals cargo.id_cargo
                        select new ViewPDFCLS
                        {
                            id_paad = paad.id_paad,
                            estado = estado.estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            id_docente = paad.docente,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre,
                            categoria_docente = categoria.categoria,
                            horas_clase = paad.horas_clase,
                            horas_investigacion = paad.horas_investigacion,
                            horas_gestion = paad.horas_gestion,
                            horas_tutorias = paad.horas_tutorias,
                            cargo = cargo.cargo,
                            firma_docente = paad.firma_docente,
                            firma_director = paad.firma_director
                        }).FirstOrDefault();
                if (info == null)
                    return null;
                info.activities = (from activity in db.Actividades
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
                                       iso = activity.iso
                                   }).ToList();
                if (IsDirector(info.id_docente))
                {
                    info.aprobado_por = (from admin in db.Administrativos where admin.rol == 2 join docente in db.Docentes on admin.docente equals docente.id_docente select docente.nombre).FirstOrDefault();
                    info.cargo_aprobado_por = "Subdirector";
                }
                else
                {
                    info.aprobado_por = (from admin in db.Administrativos where admin.rol == 3 join docente in db.Docentes on admin.docente equals docente.id_docente select docente.nombre).FirstOrDefault();
                    info.cargo_aprobado_por = "Director";
                }
            }
            return info;
        }
        /* Esta funcion devuelve el modelo para la vista de ViewPDF
         * Recibe el id del paad
         * Regresa el modelo llenado*/
        public ViewPDFCLS GetPDFInfo_IAD(int id)
        {
            ViewPDFCLS info;
            using (var db = new DB_PAAD_IADEntities())
            {
                info = (from iad in db.IADs
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
                        select new ViewPDFCLS
                        {
                            id_paad = iad.id_iad,
                            estado = estado.estado,
                            periodo = periodo.periodo,
                            carrera = carrera.carrera,
                            id_docente = iad.docente,
                            numero_empleado = docente.numero_empleado,
                            nombre_docente = docente.nombre,
                            categoria_docente = categoria.categoria,
                            horas_clase = iad.horas_clase,
                            horas_investigacion = iad.horas_investigacion,
                            horas_gestion = iad.horas_gestion,
                            horas_tutorias = iad.horas_tutorias,
                            cargo = cargo.cargo,
                            firma_docente = iad.firma_docente,
                            firma_director = iad.firma_director
                        }).FirstOrDefault();
                if (info == null)
                    return null;
                info.activities = (from activity in db.Actividades
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
                                       iso = activity.iso
                                   }).ToList();
                if (IsDirector(info.id_docente))
                {
                    info.aprobado_por = (from admin in db.Administrativos where admin.rol == 2 join docente in db.Docentes on admin.docente equals docente.id_docente select docente.nombre).FirstOrDefault();
                    info.cargo_aprobado_por = "Subdirector";
                }
                else
                {
                    info.aprobado_por = (from admin in db.Administrativos where admin.rol == 3 join docente in db.Docentes on admin.docente equals docente.id_docente select docente.nombre).FirstOrDefault();
                    info.cargo_aprobado_por = "Director";
                }
            }
            return info;
        }
        /* Esta funcion regresa un modelo con la info del periodo activo
         * Recibe el nombre de la vista y el modelo con el cual llenar la vista
         * Regresa un string con la vista */
        public InfoPeriodCLS GetInfoPeriod()
        {
            InfoPeriodCLS info = new InfoPeriodCLS();
            info.is_close = IsClose();
            info.is_close_paad = info.is_close ? true : IsClosePAAD();
            info.on_time_paad = info.is_close_paad ? false : IsOnTimePAAD();
            info.on_time_iad = info.is_close ? false : IsOnTimeIAD();
            
            return info;
        }
        /* Esta funcion revisa si la fecha actual es valida dentro del periodo activo para el llenado del paad
         * No recibe nada
         * Regresa un boleano con el resultado*/
        public bool IsOnTimePAAD()
        {
            bool isOnTime = false;
            if (IsClose())
                return isOnTime;
            using(var db=new DB_PAAD_IADEntities())
            {
                SetDateCLS dates = (from periodo in db.Periodos
                                    where periodo.activo == true
                                    select new SetDateCLS
                                    {
                                        begining = periodo.paad_inicio ?? default(DateTime),
                                        ending = periodo.paad_fin ?? default(DateTime)
                                    }).FirstOrDefault();
                if (dates != null)
                {
                    if (dates.begining.Date != null && dates.ending.Date != null)
                    {
                        DateTime today = DateTime.Today;
                        if (today.Date >= dates.begining.Date && today.Date <= dates.ending.Date)
                            isOnTime = true;
                    }
                }
                return isOnTime;
            }
        }
        /* Esta funcion revisa si la fecha actual es valida dentro del periodo activo para el llenado del paad
         * No recibe nada
         * Regresa un boleano con el resultado*/
        public bool IsClosePAAD()
        {
            bool isClose = true;
            using (var db = new DB_PAAD_IADEntities())
            {
                SetDateCLS dates = (from periodo in db.Periodos
                                    where periodo.activo == true
                                    select new SetDateCLS
                                    {
                                        begining = periodo.paad_inicio ?? default(DateTime),
                                        ending = periodo.iad_inicio ?? default(DateTime)
                                    }).FirstOrDefault();
                if (dates != null)
                {
                    if (dates.begining != default(DateTime))
                    {
                        if (dates.ending != default(DateTime))
                        {
                            if (dates.begining.Date <= DateTime.Today.Date && DateTime.Today.Date < dates.ending.Date)
                                isClose = false;
                        }
                        else if(dates.begining.Date <= DateTime.Today.Date)
                            isClose = false;
                    }
                    
                }
            }
            return isClose;
        }
        /* Esta funcion revisa si la fecha actual es valida dentro del periodo activo para el llenado del iad
         * No recibe nada
         * Regresa un boleano con el resultado*/
        public bool IsOnTimeIAD()
        {
            bool isOnTime = false;
            if (IsClose())
                return isOnTime;
            using (var db = new DB_PAAD_IADEntities())
            {
                SetDateCLS dates = (from periodo in db.Periodos
                                    where periodo.activo == true
                                    select new SetDateCLS
                                    {
                                        begining = periodo.iad_inicio ?? default(DateTime),
                                        ending = periodo.iad_fin ?? default(DateTime)
                                    }).FirstOrDefault();
                if (dates != null)
                {
                    if (dates.begining.Date != null && dates.ending.Date != null)
                    {
                        DateTime today = DateTime.Today;
                        if (today.Date >= dates.begining.Date && today.Date <= dates.ending.Date)
                            isOnTime = true;
                    }
                }
                return isOnTime;
            }
        }
        /* Esta funcion revisa y desactiva el periodo si la fecha de cierre iad ya paso
         * No recibe nada
         * Regresa un boleano, true si el sistema se cerro false si no*/
        public bool IsClose()
        {
            bool isClose = true;
            using (var db = new DB_PAAD_IADEntities())
            {
                Periodos period = ( from periodo in db.Periodos
                                    where periodo.activo == true
                                    select periodo).FirstOrDefault();
                if (period != null)
                {
                    if (period.fecha_cierre != null && DateTime.Today > period.fecha_cierre)
                    {
                        period.activo = false;
                        db.SaveChanges();
                    }
                    else
                        isClose = false;
                }
            }
            return isClose;
        }
        /* Esta funcion revisa si el id especificado pertenece a la cuenta docente del director
         * Recibe un int id
         * Regresa un boleano, true si es el director false si no*/
         public bool IsDirector(int id)
        {
            bool isDirector = false;
            using (var db = new DB_PAAD_IADEntities())
            {
                isDirector = (from admin in db.Administrativos where admin.docente == id select true).FirstOrDefault();
            }
            return isDirector;
        }
        /* Esta accion se manda llamar cuando se quiere validar las credenciales de una cuenta
         * Esta cuenta validad que la contrasena corresponda correctamente al correo
         * Recibe las credenciales
         * Regresa un booleano con el resultado de la autenticacion*/
        public bool AuthenticateCredentials(string email, string password)
        {
            using (var db = new DB_PAAD_IADEntities())
            {
                if (db.Usuarios.Where(p => p.email == email && p.password == password).Count() <= 0)
                    return false;
            }
            return true;
        }
    }
}