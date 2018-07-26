using FunerariaProyecto.Models;
using FunerariaProyecto.ViewModels;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SistemaPagos.Views.Reportes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace FunerariaProyecto.Controllers
{
    [Authorize]
    public class FacturaController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public FunerariaProyecto.Views.Reporte.DSFACTURAS DSFactura = new Views.Reporte.DSFACTURAS();
        public FunerariaProyecto.Views.Reportes.DsTotalFacturas DSTotal = new FunerariaProyecto.Views.Reportes.DsTotalFacturas();

        // GET: Factura
        [Authorize(Roles = "View")]
        public ActionResult NuevaFactura()
        {
            var facturaView = new FacturaView();
            facturaView.Cliente = new Cliente();
            facturaView.Product = new List<ProductFactura>();
            Session["FacturaView"] = facturaView;

            var listp = db.Plans.ToList();
            listp.Add(new Plan { PlanId = 0, descripcion = "[Seleccione un tipo de Plan...]" });
            listp = listp.OrderBy(p => p.descripcion).ToList();
            ViewBag.PlanId = new SelectList(listp, "PlanId", "descripcion");

            var list = db.Clientes.ToList();
            list.Add(new Cliente { ClienteId = 0, Nombre = "[Seleccione un Cliente...]" });
            list = list.OrderBy(c => c.Nombre).ToList();
            ViewBag.ClienteId = new SelectList(list, "ClienteId", "Nombre");


            return View(facturaView);
        }
        [Authorize(Roles = "Create")]
        [HttpPost]
        public ActionResult NuevaFactura(FacturaView facturaView, string dtDetalles)
        {
            if (ModelState.IsValid)
            {
                using (var db = new ApplicationDbContext())
                {


                    try
                    {

                        if (!string.IsNullOrWhiteSpace(dtDetalles))
                        {
                            var idDetalle = 0;

                            List<DetalleFactura> jarray = JsonConvert.DeserializeObject<List<DetalleFactura>>(dtDetalles);
                            int idHeader = String.IsNullOrEmpty(db.Facturas.Max(u => (int?)u.FacturaId).ToString()) ? 1 : db.Facturas.Max(u => (int?)u.FacturaId).Value + 1;
                            DetalleFactura datosDetalle = new DetalleFactura();


                            var cabecera = new Facturas()
                            {
                                FacturaId = idHeader,
                                ClienteId = facturaView.ClienteId,
                                FechaFactura = DateTime.Now,
                                FacEstatus = 1,
                            };
                            db.Facturas.Add(cabecera);


                            foreach (var item in jarray)
                            {
                                idDetalle++;
                                datosDetalle = new DetalleFactura()
                                {
                                    cantidad = item.cantidad,
                                    descripcion = item.descripcion,
                                    productoId = item.productoId,
                                    precio = item.precio,
                                    DetalleFacturaId = idDetalle,
                                    FacturaId = idHeader

                                };
                                db.DetalleFacturas.Add(datosDetalle);
                            }



                            db.SaveChanges();



                        }
                        else
                        {
                            TempData.Add("Error", "Debe la factura");


                        }
                    }
                    catch (Exception e)
                    {


                        TempData.Add("Error", e.Message);
                        return RedirectToAction("NuevaFactura");
                    }


                    return RedirectToAction("ReporteFactura");




                }
            }
            return RedirectToAction("NuevaFactura");

        }

        public ActionResult AddProduct(int ClienteId)
        {
            ViewBag.PlanID = db.Clientes.Where(c => c.ClienteId == ClienteId).FirstOrDefault().PlanId;
            ViewBag.Monto = db.Clientes.Include("plan").Where(c => c.ClienteId == ClienteId).FirstOrDefault().plan.precio;
            var listpo = db.Products.ToList();
            listpo = listpo.OrderBy(p => p.descripcion).ToList();
            ViewBag.productoId = new SelectList(listpo, "productoId", "descripcion");
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(ProductFactura productFactura)
        {
            var facturaView = Session["facturaView"] as FacturaView;

            var productoId = int.Parse(Request["ProductoId"]);
            if (productoId == 0)
            {
                var listpo = db.Products.ToList();
                listpo.Add(new Product { productoId = 0, descripcion = "[Seleccione un tipo de Descripcion...]" });
                listpo = listpo.OrderBy(p => p.descripcion).ToList();
                ViewBag.productoId = new SelectList(listpo, "productoId", "descripcion");
                ViewBag.Error = "Debe Seleccionar Un Producto";

                return View(productFactura);
            }

            var product = db.Products.Find(productoId);
            if (product == null)
            {
                var listpo = db.Products.ToList();
                listpo.Add(new Product { productoId = 0, descripcion = "[Seleccione un tipo de Descripcion...]" });
                listpo = listpo.OrderBy(p => p.descripcion).ToList();
                ViewBag.productoId = new SelectList(listpo, "productoId", "descripcion");
                ViewBag.Error = "Producto no Existe";

                return View(productFactura);
            }

            var productFacturaa = facturaView.Product.Find(p => p.productoId == productoId);
            if (productFacturaa == null)
            {

                productFactura = new ProductFactura
                {
                    productoId = product.productoId,
                    descripcion = product.descripcion,
                    precioo = productFactura.precioo,
                    cantidad = float.Parse(Request["cantidad"]),
                    ClienteId = productFactura.ClienteId,
                    precio = productFactura.precioo,
                    stock = 0
                };
                facturaView.Product.Add(productFactura);
            }
            else
            {
                productFacturaa.cantidad += float.Parse(Request["cantidad"]);
            }
            //else
            //{
            //    productFactura.cantidad += float.Parse(Request["cantidad"]);
            //}


            var listp = db.Plans.ToList();
            listp.Add(new Plan { PlanId = 0, descripcion = "[Seleccione un tipo de Plan...]" });
            listp = listp.OrderBy(p => p.descripcion).ToList();
            ViewBag.PlanId = new SelectList(listp, "PlanId", "descripcion");

            var list = db.Clientes.ToList();
            list.Add(new Cliente { ClienteId = 0, Nombre = "[Seleccione un tipo de Cliente...]" });
            list = list.OrderBy(c => c.Nombre).ToList();
            ViewBag.ClienteId = new SelectList(list, "ClienteId", "Nombre");

            return View("NuevaFactura", facturaView);
        }
        [Authorize(Roles = "View")]
        [HttpPost]
        public ActionResult Buscar()
        {
            return RedirectToAction("ListadoFactura");
        }

        public ActionResult ListadoFactura(int? clienteId, int? sucursalid, int? facEstatus)
        {


            var result = from r in db.Facturas
                         join e in db.Clientes on r.ClienteId.ToString() equals e.ClienteId.ToString()
                         join s in db.DetalleFacturas on r.FacturaId.ToString() equals s.FacturaId.ToString()
                         where (r.ClienteId == clienteId || clienteId == null && e.SucursalId == sucursalid || sucursalid == null && r.FacEstatus == facEstatus || facEstatus == null)
                         select new FacturaView()
                         {
                             ClienteId = int.Parse(e.ClienteId.ToString()),
                             ProductoId = s.productoId,
                             Cliente = e,
                             FacturaId = r.FacturaId
                         };



            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "clienteId", "Nombre");
            ViewBag.Sucursal = new SelectList(db.Sucursals.ToList(), "SucursalId", "Nombre");
            ViewBag.Estado = new SelectList(db.Database.SqlQuery<FacturasEstados>("Select FacEstatus,(case when FacEstatus = 1 then 'Pendiente' else 'Aprobada' end) as FacEstatusDesc  from Facturas group by FacEstatus").ToList(), "FacEstatus", "FacEstatusDesc");


            return View(result.AsParallel());
        }
        [Authorize(Roles = "User")]
        public ActionResult AprobarFactura(int? id)
        {
            var fac = from fact in db.Facturas
                      where fact.FacturaId == id
                      select fact;
            foreach (Facturas facs in fac)
            {
                facs.FacEstatus = 2;
            }

            db.SaveChanges();
            return RedirectToAction("ListadoFactura");
        }

        [HttpPost]
        public JsonResult GetClientes()
        {

            var getPoliticas = from r in db.Clientes
                               join e in db.Plans on r.PlanId.ToString() equals e.PlanId.ToString()
                               join s in db.Sucursals on r.SucursalId.ToString() equals s.SucursalId.ToString()
                               // where ((r.ClienteId == clienteid || clienteid == null) && (e.SucursalId == sucursalid || sucursalid == null))
                               select new FacturaView()
                               {
                                   ClienteNombre = r.Nombre,
                                   ClienteId = r.ClienteId,
                                   ClienteCodigo = r.ClienteCodigo,
                                   ClienteSucursal = s.Nombre,
                                   ClientePlan = e.descripcion,
                                   ClienteCedula = r.Cedula,
                                   ClienteDireccion = r.Direccion,
                                   ClienteTelefono = r.Telefono,
                                   ClienteFecha = r.Fecha
                               };


            return Json(getPoliticas.OrderBy(x => x.ClienteCodigo), JsonRequestBehavior.DenyGet);

        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public JsonResult GetFacturas(int? clienteid, int? sucursalid, DateTime desde, DateTime hasta, int? facEstatus)
        {

            var getPoliticas = from r in db.Facturas
                               join e in db.Clientes on r.ClienteId.ToString() equals e.ClienteId.ToString()
                               join s in db.DetalleFacturas on r.FacturaId.ToString() equals s.FacturaId.ToString()
                               where ((r.ClienteId == clienteid || String.IsNullOrEmpty(clienteid.ToString())) && (e.SucursalId == sucursalid || String.IsNullOrEmpty(sucursalid.ToString()) && (r.FacEstatus == facEstatus || String.IsNullOrEmpty(facEstatus.ToString()))
                               && (r.FechaFactura >= desde.Date) && (r.FechaFactura <= hasta.Date)))
                               select new FacturaView()
                               {
                                   ProductoId = s.productoId,
                                   ClienteNombre = e.Nombre,
                                   ClienteSucursal = e.sucursal.Nombre,
                                   FacturaId = r.FacturaId,
                                   ClientePlan = "",
                                   cantidad = s.cantidad,
                                   precio = s.precio,
                                   estado = r.FacEstatus.ToString().Equals("1") ? "Pendiente" : "Aprobada",


                               };


            return Json(getPoliticas, JsonRequestBehavior.DenyGet);

        }
        [Authorize(Roles = "View")]
        public ActionResult ReporteFactura()
        {

            try
            {
                ReportViewer reportViewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100)
                };

                var cabecera = db.Facturas.Max(fac => fac.FacturaId);

                string direccion = Server.MapPath("/Views/Reportes/Factura.rdlc");
                reportViewer.LocalReport.ReportPath = direccion;

                SetFactura(cabecera);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DsFacturas", DSFactura.Tables[0]));

                reportViewer.ServerReport.Refresh();
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(1000);
                reportViewer.Height = Unit.Percentage(1000);
                ViewBag.ReportView = reportViewer;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return View();

        }

        public ActionResult ReporteGeneral(int? clienteId, int? sucursalId, string fechade, string fechaha, int? facestatus)
        {

            try
            {
                ReportViewer reportViewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100)
                };



                string direccion = Server.MapPath("/Views/Reportes/FacturasRealizadas.rdlc");
                reportViewer.LocalReport.ReportPath = direccion;

                SetFacturaGeneral(clienteId, sucursalId, fechade, fechaha, facestatus);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DsTotalFacturas", DSTotal.Tables[0]));

                reportViewer.ServerReport.Refresh();
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(1000);
                reportViewer.Height = Unit.Percentage(1000);
                ViewBag.ReportView = reportViewer;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return View();

        }
        [Authorize(Roles = "User")]
        public ActionResult ReimprimirFactura(int id)
        {

            try
            {
                ReportViewer reportViewer = new ReportViewer()
                {
                    ProcessingMode = ProcessingMode.Local,
                    SizeToReportContent = true,
                    Width = Unit.Percentage(100),
                    Height = Unit.Percentage(100)
                };

                var cabecera = id;

                string direccion = Server.MapPath("/Views/Reportes/Reimpresion.rdlc");
                reportViewer.LocalReport.ReportPath = direccion;

                SetFactura(cabecera);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DsFacturas", DSFactura.Tables[0]));

                reportViewer.ServerReport.Refresh();
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(1000);
                reportViewer.Height = Unit.Percentage(1000);
                ViewBag.ReportView = reportViewer;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
            }
            return View();

        }

        private void SetFactura(int cabecera)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionStringDB()))
            {
                SqlCommand cmd = new SqlCommand()
                {
                    CommandText = "FacturaCargar",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@Facturaid", SqlDbType.VarChar).Value = cabecera;

                cmd.Connection = Connection;
                Connection.Open();
                SqlDataAdapter dt = new SqlDataAdapter(cmd);
                dt.Fill(DSFactura, DSFactura.FacturaCargar.TableName);
            }
        }

        private void SetFacturaGeneral(int? clienteId, int? sucursalId, string fechade, string fechaha, int? facestatus)
        {
            using (SqlConnection Connection = new SqlConnection(ConnectionStringDB()))
            {
                SqlCommand cmd = new SqlCommand()
                {
                    CommandText = "CargarTotalFacturas",
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("@clienteId", SqlDbType.VarChar).Value = clienteId;
                cmd.Parameters.Add("@sucursalId", SqlDbType.VarChar).Value = sucursalId;
                cmd.Parameters.Add("@fechade", SqlDbType.VarChar).Value = fechade;
                cmd.Parameters.Add("@fechaha", SqlDbType.VarChar).Value = fechaha;
                cmd.Parameters.Add("@facestatus", SqlDbType.Int).Value = facestatus;




                cmd.Connection = Connection;
                Connection.Open();
                SqlDataAdapter dt = new SqlDataAdapter(cmd);
                dt.Fill(DSTotal, DSTotal.CargarTotalFacturas.TableName);
            }
        }

        private string ConnectionStringDB()
        {
            string conn = ConfigurationManager.ConnectionStrings["FunerariaProyectoContext"].ToString();

            return conn;
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