using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FunerariaProyecto.Models;

namespace FunerariaProyecto.Controllers
{
    [Authorize]
    public class DetallePagoesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: DetallePagoes
        public ActionResult Index()
        {
            var detallePagoes = db.DetallePagoes.Include(d => d.pago).Include(d => d.products);
            return View(detallePagoes.ToList());
        }

        // GET: DetallePagoes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePago detallePago = db.DetallePagoes.Find(id);
            if (detallePago == null)
            {
                return HttpNotFound();
            }
            return View(detallePago);
        }

        // GET: DetallePagoes/Create
        public ActionResult Create()
        {
            ViewBag.PagoId = new SelectList(db.Pagoes, "PagoId", "comentario");
            ViewBag.PlanId = new SelectList(db.Plans, "PlanId", "descripcion");
            ViewBag.productoId = new SelectList(db.Products, "productoId", "descripcion");
            return View();
        }

        // POST: DetallePagoes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DetallePagoId,PagoId,productoId,PlanId,descripcion,cantidad,precio")] DetallePago detallePago)
        {
            if (ModelState.IsValid)
            {
                db.DetallePagoes.Add(detallePago);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PagoId = new SelectList(db.Pagoes, "PagoId", "comentario", detallePago.PagoId);
            ViewBag.productoId = new SelectList(db.Products, "productoId", "descripcion", detallePago.productoId);
            return View(detallePago);
        }

        // GET: DetallePagoes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePago detallePago = db.DetallePagoes.Find(id);
            if (detallePago == null)
            {
                return HttpNotFound();
            }
            ViewBag.PagoId = new SelectList(db.Pagoes, "PagoId", "comentario", detallePago.PagoId);
            ViewBag.productoId = new SelectList(db.Products, "productoId", "descripcion", detallePago.productoId);
            return View(detallePago);
        }

        // POST: DetallePagoes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DetallePagoId,PagoId,productoId,PlanId,descripcion,cantidad,precio")] DetallePago detallePago)
        {
            if (ModelState.IsValid)
            {
                db.Entry(detallePago).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PagoId = new SelectList(db.Pagoes, "PagoId", "comentario", detallePago.PagoId);
            ViewBag.productoId = new SelectList(db.Products, "productoId", "descripcion", detallePago.productoId);
            return View(detallePago);
        }

        // GET: DetallePagoes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            DetallePago detallePago = db.DetallePagoes.Find(id);
            if (detallePago == null)
            {
                return HttpNotFound();
            }
            return View(detallePago);
        }

        // POST: DetallePagoes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DetallePago detallePago = db.DetallePagoes.Find(id);
            db.DetallePagoes.Remove(detallePago);
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
