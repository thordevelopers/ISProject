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

        public VisualizarPAADCLS FillPAAD(int id)
        {

            VisualizarPAADCLS model = new VisualizarPAADCLS();
            using (var db = new DB_PAAD_IADEntities())
            {
                Docentes doc = ((Docentes)Session["user"]);
                PAADs paad = db.PAADs.Where(p => p.docente == doc.id_docentes && p.id_paad == id).FirstOrDefault();
                if (paad == null)
                {
                    return null;
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