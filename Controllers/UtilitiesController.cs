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
        public ActionResult ViewPDF(int id)
        {
            ViewPDFCLS paad = GetViewPDFInfo(id);
            //El view as paad regresa la vista espeficada llenada con el modelo dado como un documento pdf segun los argumentos mandado entre las llaves
            return new ViewAsPdf("ViewPDF",paad)
            {
                PageOrientation = Rotativa.Options.Orientation.Landscape,
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
            };
        }
        /* Esta funcion devuelve el modelo para la vista de ViewPDF
         * Recibe el id del paad
         * Regresa el modelo llenado*/
        public ViewPDFCLS GetViewPDFInfo(int id)
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
                                       porcentaje_inicial = activity.porcentaje_inicial,
                                       porcentaje_final = activity.porcentaje_final,
                                       cacei = activity.cacei,
                                       cuerpo_academico = activity.cuerpo_academico,
                                       iso = activity.iso
                                   }).ToList();
                info.director = (from docente in db.Docentes
                                 where docente.isdirector == true
                                 select docente.nombre).FirstOrDefault();

            }
            return info;
        }

    }
}