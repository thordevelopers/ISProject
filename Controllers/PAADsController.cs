using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace ISProject.Models
{
    public class PAADsController : Controller
    {
        private DB_PAAD_IADEntities db = new DB_PAAD_IADEntities();

        // GET: PAADs
        public ActionResult Index()
        {
            var pAADs = db.PAADs.Include(p => p.Cargos).Include(p => p.Carreras).Include(p => p.Categorias).Include(p => p.Docentes).Include(p => p.Estados).Include(p => p.Periodos);
            return View(pAADs.ToList());
        }

        // GET: PAADs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAADs pAADs = db.PAADs.Find(id);
            if (pAADs == null)
            {
                return HttpNotFound();
            }
            return View(pAADs);
        }

        // GET: PAADs/Create
        public ActionResult Create()
        {
            ViewBag.cargo = new SelectList(db.Cargos, "id_cargo", "cargo");
            ViewBag.carrera = new SelectList(db.Carreras, "id_carrera", "carrera");
            ViewBag.categoria_docente = new SelectList(db.Categorias, "id_categoria", "categoria");
            ViewBag.numero_empleado = new SelectList(db.Docentes, "numero_empleado", "nombre");
            ViewBag.id_paad = new SelectList(db.Estados, "id_estado", "estado");
            ViewBag.periodo = new SelectList(db.Periodos, "id_periodo", "periodo");
            return View();
        }

        // POST: PAADs/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id_paad,estado,periodo,carrera,numero_empleado,categoria_docente,horas_clase,horas_investigacion,horas_gestion,horas_tutorias,cargo,firma_docente,firma_director")] PAADs pAADs)
        {
            if (ModelState.IsValid)
            {
                db.PAADs.Add(pAADs);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.cargo = new SelectList(db.Cargos, "id_cargo", "cargo", pAADs.cargo);
            ViewBag.carrera = new SelectList(db.Carreras, "id_carrera", "carrera", pAADs.carrera);
            ViewBag.categoria_docente = new SelectList(db.Categorias, "id_categoria", "categoria", pAADs.categoria_docente);
            ViewBag.numero_empleado = new SelectList(db.Docentes, "numero_empleado", "nombre", pAADs.numero_empleado);
            ViewBag.id_paad = new SelectList(db.Estados, "id_estado", "estado", pAADs.id_paad);
            ViewBag.periodo = new SelectList(db.Periodos, "id_periodo", "periodo", pAADs.periodo);
            return View(pAADs);
        }

        // GET: PAADs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAADs pAADs = db.PAADs.Find(id);
            if (pAADs == null)
            {
                return HttpNotFound();
            }
            ViewBag.cargo = new SelectList(db.Cargos, "id_cargo", "cargo", pAADs.cargo);
            ViewBag.carrera = new SelectList(db.Carreras, "id_carrera", "carrera", pAADs.carrera);
            ViewBag.categoria_docente = new SelectList(db.Categorias, "id_categoria", "categoria", pAADs.categoria_docente);
            ViewBag.numero_empleado = new SelectList(db.Docentes, "numero_empleado", "nombre", pAADs.numero_empleado);
            ViewBag.id_paad = new SelectList(db.Estados, "id_estado", "estado", pAADs.id_paad);
            ViewBag.periodo = new SelectList(db.Periodos, "id_periodo", "periodo", pAADs.periodo);
            return View(pAADs);
        }

        // POST: PAADs/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id_paad,estado,periodo,carrera,numero_empleado,categoria_docente,horas_clase,horas_investigacion,horas_gestion,horas_tutorias,cargo,firma_docente,firma_director")] PAADs pAADs)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pAADs).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.cargo = new SelectList(db.Cargos, "id_cargo", "cargo", pAADs.cargo);
            ViewBag.carrera = new SelectList(db.Carreras, "id_carrera", "carrera", pAADs.carrera);
            ViewBag.categoria_docente = new SelectList(db.Categorias, "id_categoria", "categoria", pAADs.categoria_docente);
            ViewBag.numero_empleado = new SelectList(db.Docentes, "numero_empleado", "nombre", pAADs.numero_empleado);
            ViewBag.id_paad = new SelectList(db.Estados, "id_estado", "estado", pAADs.id_paad);
            ViewBag.periodo = new SelectList(db.Periodos, "id_periodo", "periodo", pAADs.periodo);
            return View(pAADs);
        }

        // GET: PAADs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PAADs pAADs = db.PAADs.Find(id);
            if (pAADs == null)
            {
                return HttpNotFound();
            }
            return View(pAADs);
        }

        // POST: PAADs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PAADs pAADs = db.PAADs.Find(id);
            db.PAADs.Remove(pAADs);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
