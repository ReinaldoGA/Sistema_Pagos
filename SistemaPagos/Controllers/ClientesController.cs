using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FunerariaProyecto.Models;
using Newtonsoft.Json;

namespace FunerariaProyecto.Controllers
{
    public class ClientesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [Authorize(Roles = "View")]
        // GET: Clientes
        public ActionResult Index()
        {
            var clientes = db.Clientes.Include(c => c.plan).Include(c => c.sucursal);
            //ViewBag.Clientes = db.ClienteDetalle.ToList().GroupBy(c=>c.ClienteID);
            return View(clientes.OrderBy(c=> c.ClienteCodigo).ToList());
        }

        [HttpPost]
        public JsonResult GetClientes()
        {
            var clientes = db.Clientes.Include(c => c.plan).Include(c => c.sucursal);
            /*db.Clientes.Include(c => c.plan).Include(c => c.sucursal);*/
            var clientese = clientes.OrderBy(c => c.ClienteCodigo).ToList();

            //where ((r.ClienteId == clienteid || clienteid == null) && (e.SucursalId == sucursalid || sucursalid == null))

            return Json(clientese, JsonRequestBehavior.DenyGet);

        }

        // GET: Clientes/Details/5
        [Authorize(Roles = "View")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ViewBag.Nombre = db.Clientes.Find(id);

            if (ViewBag.Nombre == null)
            {
                return HttpNotFound();
            }
            
            var clientes = db.ClienteDetalle.Where(c => c.ClienteID == id).ToList();
            return View(clientes);
        }

        // GET: Clientes/Create
        [Authorize(Roles = "Create")]
        public ActionResult Create()
        {
            ViewBag.PlanId = new SelectList(db.Plans, "PlanId", "descripcion");
            ViewBag.SucursalId = new SelectList(db.Sucursals, "SucursalId", "Nombre");
            return View();
        }

        // POST: Clientes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Cliente cliente, string dtDetalle)
        {
            if (ModelState.IsValid)
            {
                db.Clientes.Add(cliente);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(dtDetalle)) 
                {
                   
                    List<ClienteDetalle> jarray = JsonConvert.DeserializeObject<List<ClienteDetalle>>(dtDetalle);
                     foreach(var item in jarray)
                     {
                         int? idHeader = String.IsNullOrEmpty(db.ClienteDetalle.Max(u => (int?)u.detalleClienteID).ToString()) ? 1 : db.ClienteDetalle.Max(u => (int?)u.detalleClienteID).Value + 1;
                        var detalle = new ClienteDetalle()
                        {
                            ClienteID = cliente.ClienteId,
                            detalleClienteID = int.Parse(idHeader.ToString()),
                            Nombre = item.Nombre,
                            Parentezco = item.Parentezco,
                            OtrosDatos = item.OtrosDatos

                        };
                        db.ClienteDetalle.Add(detalle);
                         db.SaveChanges();
                     }

               }
                
               
                return RedirectToAction("Index");
            }

            ViewBag.PlanId = new SelectList(db.Plans, "PlanId", "descripcion", cliente.PlanId);
            ViewBag.SucursalId = new SelectList(db.Sucursals, "SucursalId", "Nombre", cliente.SucursalId);
            return View(cliente);
        }

        // GET: Clientes/Edit/5
        [Authorize(Roles = "Edit")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            ViewBag.PlanId = new SelectList(db.Plans.ToList(), "PlanId", "descripcion", cliente.PlanId);
            ViewBag.SucursalId = new SelectList(db.Sucursals.ToList(), "SucursalId", "Nombre", cliente.SucursalId);

            List<ClienteDetalle> detalle = db.ClienteDetalle.Where(x=>x.ClienteID == id).ToList();

            ViewBag.ClienteDetalle = detalle;
            return View(cliente);
        }

        // POST: Clientes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Cliente cliente,string dtDetalle)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cliente).State = EntityState.Modified;

                List<ClienteDetalle> detalle = db.ClienteDetalle.Where(x => x.ClienteID == cliente.ClienteId).ToList();
                if (detalle.Count != 0)
                {
                    for (int i = 0; i < detalle.Count; i++)
                    {
                        db.ClienteDetalle.Remove(detalle[i]);
                        db.SaveChanges();
                    }
                }
                if (!string.IsNullOrEmpty(dtDetalle))
                {

                    List<ClienteDetalle> jarray = JsonConvert.DeserializeObject<List<ClienteDetalle>>(dtDetalle);
                    foreach (var item in jarray)
                    {
                        int? idHeader = String.IsNullOrEmpty(db.ClienteDetalle.Max(u => (int?)u.detalleClienteID).ToString()) ? 1 : db.ClienteDetalle.Max(u => (int?)u.detalleClienteID).Value + 1;
                        var detalles = new ClienteDetalle()
                        {
                            ClienteID = cliente.ClienteId,
                            detalleClienteID = int.Parse(idHeader.ToString()),
                            Nombre = item.Nombre,
                            Parentezco = item.Parentezco,
                            OtrosDatos = item.OtrosDatos

                        };
                        db.ClienteDetalle.Add(detalles);
                        db.SaveChanges();
                    }

                }

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PlanId = new SelectList(db.Plans, "PlanId", "descripcion", cliente.PlanId);
            ViewBag.SucursalId = new SelectList(db.Sucursals, "SucursalId", "Nombre", cliente.SucursalId);
            return View(cliente);
        }

        // GET: Clientes/Delete/5
        [Authorize(Roles = "Delete")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cliente cliente = db.Clientes.Find(id);
            if (cliente == null)
            {
                return HttpNotFound();
            }
            return View(cliente);
        }

        // POST: Clientes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cliente cliente = db.Clientes.Find(id);
            db.Clientes.Remove(cliente);
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
