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
    public class DocentesController : Controller
    {
        private DB_PAAD_IADEntities db = new DB_PAAD_IADEntities();

        // GET: Docentes
        public ActionResult Index()
        {
            return View(db.Docentes.ToList());
        }

        // GET: Docentes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docentes docentes = db.Docentes.Find(id);
            if (docentes == null)
            {
                return HttpNotFound();
            }
            return View(docentes);
        }

        // GET: Docentes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Docentes/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "numero_empleado,nombre,correo")] Docentes docentes)
        {
            if (ModelState.IsValid)
            {
                db.Docentes.Add(docentes);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(docentes);
        }

        // GET: Docentes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docentes docentes = db.Docentes.Find(id);
            if (docentes == null)
            {
                return HttpNotFound();
            }
            return View(docentes);
        }

        // POST: Docentes/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "numero_empleado,nombre,correo")] Docentes docentes)
        {
            if (ModelState.IsValid)
            {
                db.Entry(docentes).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(docentes);
        }

        // GET: Docentes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Docentes docentes = db.Docentes.Find(id);
            if (docentes == null)
            {
                return HttpNotFound();
            }
            return View(docentes);
        }

        // POST: Docentes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Docentes docentes = db.Docentes.Find(id);
            db.Docentes.Remove(docentes);
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
