using FunerariaProyecto.Models;
using FunerariaProyecto.ViewModels;
using Microsoft.Reporting.WebForms;
using Newtonsoft.Json;
using SistemaPagos.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using IronPdf;

namespace FunerariaProyecto.Controllers
{
    [Authorize(Roles ="Facturacion")]
    public class FacturaController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        public Views.Reporte.DSFACTURAS DSFactura = new Views.Reporte.DSFACTURAS();
        public Views.Reportes.DsTotalFacturas DSTotal = new Views.Reportes.DsTotalFacturas();
        private IList<Stream> pages = new List<Stream>();
        private int currentPageIndex;


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

            ClientesViewModel clientes =  new ClientesViewModel();
            var list = from r in db.Clientes
                       join e in db.Plans on r.PlanId.ToString() equals e.PlanId.ToString()
                       join s in db.Sucursals on r.SucursalId.ToString() equals s.SucursalId.ToString()
                       // where ((r.ClienteId == clienteid || clienteid == null) && (e.SucursalId == sucursalid || sucursalid == null))
                       select new ClientesViewModel()
                       {
                            Nombre = r.ClienteCodigo +"--" +s.Nombre +"--" + r.Nombre,
                            ClienteId = r.ClienteId,
                       };


            var lista = list.ToList();
            lista.Add((new ClientesViewModel { ClienteId = 0, Nombre = "[Seleccione un Cliente...]" }));
            ViewBag.ClienteId = new SelectList(lista, "ClienteId", "Nombre",0);


            return View(facturaView);
        }
        [Authorize(Roles = "Create")]
        [HttpPost]
        public ActionResult NuevaFactura(FacturaView facturaView, string dtDetalles)
        {

            //CultureInfo DoTime = CultureInfo.CreateSpecificCulture("es-DO");
            //var hoy = Convert.ToString();
            var ukTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
           // DateTime Today = TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, ukTimeZone);
            //var DoTime = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            //var Today = ukTime.AddMinutes(-240);
            //DateTime Today = DateTime.Now;

            var info = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");
            DateTimeOffset localServerTime = DateTimeOffset.Now;
            DateTimeOffset istambulTime = TimeZoneInfo.ConvertTime(localServerTime, info);
            var Today = istambulTime.DateTime;
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
                                ClienteId = facturaView.ClienteId2,
                                FechaFactura = Today,
                                FacEstatus = 1,
                                UserName = User.Identity.Name
                            };
                            db.Facturas.Add(cabecera);


                            foreach (var item in jarray)
                            {
                                idDetalle++;
                                datosDetalle = new DetalleFactura()
                                {
                                    cantidad = 1,
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

                    HttpBrowserCapabilitiesBase Navegador  = Request.Browser;
                    string SistemaOperativo = Navegador.Platform;



                    if (SistemaOperativo.Contains("Win"))
                    {

                        return RedirectToAction("ReporteFactura");
                    }
                    else
                    {
                        return RedirectToAction("Conduce80MM");

                    }
                }
            }
            return RedirectToAction("NuevaFactura");

        }
        [Authorize(Roles = "Create")]
        public ActionResult AddProduct(string dtDetalle, int ClienteId)
        {
            List<Product> jarray = JsonConvert.DeserializeObject<List<Product>>(dtDetalle);
           
          // var  detallesjarray2 = detallesjarray.Where(x=>jarray.productoId == x.productoId).ToList();
            var productos = from pr in db.Products
                            join tr in db.DetalleFacturas on pr.productoId equals tr.productoId
                            where pr.productoId == tr.productoId
                            select pr;

            ViewBag.PlanID = db.Clientes.Where(c => c.ClienteId == ClienteId).FirstOrDefault().PlanId;
            ViewBag.Monto = db.Clientes.Include("plan").Where(c => c.ClienteId == ClienteId).FirstOrDefault().plan.precio;
            var  tiempo_inscripcion =  db.Clientes.Where(x => x.ClienteId == ClienteId).FirstOrDefault().Fecha;
            var proidlast = from r in db.Facturas
                            join e in db.Clientes on r.ClienteId.ToString() equals e.ClienteId.ToString()
                            join s in db.DetalleFacturas on r.FacturaId.ToString() equals s.FacturaId.ToString()
                            join p in db.Products on s.productoId equals p.productoId
                            where (r.ClienteId == ClienteId  & r.FacEstatus != 0)
                            select p.productoId;

            if (proidlast.Count() == 0)
            {
                if (jarray == null)
                {
                    var info = TimeZoneInfo.FindSystemTimeZoneById("SA Western Standard Time");

                    DateTimeOffset localServerTime = DateTimeOffset.Now;
                    DateTimeFormatInfo dtinfo = new CultureInfo("es-Do", false).DateTimeFormat;
                    DateTimeOffset istambulTime = TimeZoneInfo.ConvertTime(localServerTime, info);
                    var Today = istambulTime.DateTime;
                    var mes = dtinfo.GetMonthName(DateTime.Now.Month);
                    var anio = DateTime.Now.Year.ToString();


                    if ((Today - tiempo_inscripcion).TotalDays > 30 & (Today - tiempo_inscripcion).TotalDays > 30.99)
                    {
                        var listpo = db.Products.OrderByDescending(x => x.productoId).Where(d => d.descripcion.Contains(mes) & d.descripcion.Contains(anio)).ToList().Take(1);
                        ViewBag.productoId = new SelectList(listpo, "productoId", "descripcion");
                    }
                    else
                    {
                        var mespago = dtinfo.GetMonthName(tiempo_inscripcion.Month);
                        var listpo = db.Products.OrderByDescending(x => x.productoId).Where(d => d.descripcion.Contains(mespago) & d.descripcion.Contains(tiempo_inscripcion.Year.ToString())).ToList().Take(1);
                        ViewBag.productoId = new SelectList(listpo, "productoId", "descripcion");
                    }
                    //listpo = listpo.OrderBy(p => p.descripcion).ToList();
                    
                    return View();
                }
                else
                {
                    var listpo = db.Products.ToList();

                    var listaaa = listpo.Where(x=> jarray.FirstOrDefault().productoId != x.productoId & x.productoId > jarray.Last().productoId).ToList().Take(1);
                    ViewBag.productoId = new SelectList(listaaa, "productoId", "descripcion");

                    ////listpo = listpo.OrderBy(p => p.p).ToList();
                    //ViewBag.productoId = new SelectList(listpo, "ProductoId", "ProductName");
                    return View();
                }
            }
            else
            {
                if (jarray == null)
                {
                    var listpo = from p in db.Products
                                 where !proidlast.Contains(p.productoId)
                                 select p;
                    ViewBag.productoId = new SelectList(listpo.Where(x => x.productoId  > (proidlast.FirstOrDefault())).OrderBy(x=>x.productoId).ToList().Take(1), "productoId", "descripcion");

                    return View();
                }
                else
                {

                    var listpo = db.Products.ToList();
                    var lispopo = from p in db.Products
                                  select p;

                    var listaaa = listpo.Where(x => !jarray.Select(j => j.productoId).Contains(x.productoId) & !proidlast.ToList().Contains(x.productoId) & x.productoId > jarray.Last().productoId);
                    ViewBag.productoId = new SelectList(listaaa.OrderBy(x => x.productoId).ToList().Take(1), "productoId", "descripcion");
                    return View();
                }
            }
        }

        [HttpPost]
        [Authorize(Roles = "Create")]
        public ActionResult AddProduct(ProductFactura productFactura)
        {
            var facturaView = Session["facturaView"] as FacturaView;

            var productoId = int.Parse(Request["ProductoId"]);
            if (productoId == 0)
            {
                var listpo = db.Products.ToList();
                listpo.Add(new Product { productoId = 0, descripcion = "[Seleccione un tipo de Descripcion...]" });
                listpo = listpo.OrderByDescending(p => p.descripcion).ToList();
                ViewBag.productoId = new SelectList(listpo, "productoId", "descripcion");
                ViewBag.Error = "Debe Seleccionar Un Producto";

                return View(productFactura);
            }

            var product = db.Products.Find(productoId);
            if (product == null)
            {
                var listpo = db.Products.ToList();
                listpo.Add(new Product { productoId = 0, descripcion = "[Seleccione un tipo de Descripcion...]" });
                listpo = listpo.OrderByDescending(p => p.descripcion).ToList();
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

            var listp = db.Plans.ToList();
            listp.Add(new Plan { PlanId = 0, descripcion = "[Seleccione un tipo de Plan...]" });
            listp = listp.OrderByDescending(p => p.descripcion).ToList();
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
            ViewBag.Estado = new SelectList(db.Database.SqlQuery<FacturasEstados>("Select FacEstatus,(case when FacEstatus = 1 then 'Digitada' else 'Anulada' end) as FacEstatusDesc  from Facturas group by FacEstatus").ToList(), "FacEstatus", "FacEstatusDesc");


            return View(result.AsParallel());
        }


        public ActionResult Cuadre()
        {
            ViewBag.Clientes = new SelectList(db.Clientes.ToList(), "clienteId", "Nombre");
            ViewBag.Sucursal = new SelectList(db.Sucursals.ToList(), "SucursalId", "Nombre");
            ViewBag.Estado = new SelectList(db.Database.SqlQuery<FacturasEstados>("Select FacEstatus,(case when FacEstatus = 1 then 'Digitada' else 'Anulada' end) as FacEstatusDesc  from Facturas group by FacEstatus").ToList(), "FacEstatus", "FacEstatusDesc");
            ViewBag.Users = new SelectList(db.Database.SqlQuery<FacturaView>("Select distinct UserName from Facturas").ToList(), "UserName", "UserName");

            return View();
        }
        [Authorize(Roles = "Anular")]
        public ActionResult AnularFactura(int? id)
        {
            var fac = from fact in db.Facturas
                      where fact.FacturaId == id
                      select fact;
            foreach (Facturas facs in fac)
            {
                facs.FacEstatus = 0;
            }

            db.SaveChanges();
            return RedirectToAction("ListadoFactura");
        }

        [Authorize(Roles = "Anular")]
        public ActionResult AnularFactura2(int? id)
        {
            var fac = from fact in db.Facturas
                      where fact.FacturaId == id
                      select fact;
            foreach (Facturas facs in fac)
            {
                facs.FacEstatus = 0;
            }

            db.SaveChanges();
            return RedirectToAction("Cuadre");
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

            /*var getPoliticas = from r in db.Facturas
                                join e in db.Clientes on r.ClienteId.ToString() equals e.ClienteId.ToString()
                                join s in db.DetalleFacturas on r.FacturaId.ToString() equals s.FacturaId.ToString()
                                join p in db.Plans on e.PlanId.ToString() equals p.PlanId.ToString()

                                where (((r.ClienteId == clienteid || String.IsNullOrEmpty(clienteid.ToString())) && (e.SucursalId == sucursalid || String.IsNullOrEmpty(sucursalid.ToString())) && (r.FacEstatus == facEstatus || String.IsNullOrEmpty(facEstatus.ToString())))
                                )
                                new FacturaView()
                                {
                                    ProductoId = s.productoId,
                                    ClienteNombre = e.Nombre,
                                    ClienteSucursal = e.sucursal.Nombre,
                                    FacturaId = r.FacturaId,
                                    FacturaNo = e.PlanId.ToString() + e.ClienteCodigo.ToString() + r.FacturaId.ToString(),
                                    ClientePlan = p.descripcion,
                                    ClienteFecha = r.FechaFactura,
                                    precio = s.precio,

                                    estado = r.FacEstatus.ToString().Equals("1") ? "Digitada" : "Anulada",


                                };*/
            //var a = db.Database.SqlQuery<FacturaView>(String.Format("select df.ProductoId,c.Nombre as ClienteNombre,s.Nombre as ClienteSucursal,(convert(varchar,p.planId) + c.ClienteCodigo + convert(varchar,f.FacturaId)) as FacturaNo,p.descripcion as ClientePlan,f.FechaFactura as ClienteFecha,df.precio as precio,(case when FacEstatus = 1 then 'Digitada' else 'Anulada' end) as estado from Facturas f inner join Clientes c on f.clienteid = c.clienteid inner join DetalleFacturas df on f.facturaid = df.FacturaId inner join Plans p on c.planid = p.planid inner join Sucursals s on c.SucursalId = s.SucursalId where (   f.clienteid = {0} or f.clienteid is null ) and ( c.sucursalId is null or c.sucursalid = {1} ) and (f.FacEstatus = {2} or f.FacEstatus is null) and (cast(f.FechaFactura as date) between {3} and {4})", (clienteid == null ? 0 : clienteid), (sucursalid == null ? 0 : sucursalid), (facEstatus == null ? 0:facEstatus),desde.Date,hasta.Date)).ToList();
            //var a = getPoliticas.Where( x=> x.ClienteFecha >= desde.Date && x.ClienteFecha<= hasta.Date);
            string exampleSQL = "select df.descripcion as Producto,f.FacturaId,df.ProductoId,c.Nombre as ClienteNombre,s.Nombre as ClienteSucursal,(convert(varchar, p.planId) + c.ClienteCodigo + convert(varchar, f.FacturaId)) as FacturaNo,p.descripcion as ClientePlan,f.FechaFactura as ClienteFecha,df.precio as precio,(case when FacEstatus = 1 then 'Digitada' else 'Anulada' end) as estado from Facturas f inner join Clientes c on f.clienteid = c.clienteid inner join DetalleFacturas df on f.facturaid = df.FacturaId inner join Plans p on c.planid = p.planid inner join Sucursals s on c.SucursalId = s.SucursalId where (@clienteid = 0   OR f.clienteid = @clienteid ) and(@sucursalid = 0  or c.sucursalid = @sucursalid) and(@facestatus = 10 OR f.FacEstatus = @facestatus) and(cast(f.FechaFactura as date) between @desde and @hasta)";
            SqlConnection connection = new SqlConnection(ConnectionStringDB());
            SqlCommand command = new SqlCommand(exampleSQL, connection);
            DataSet ds = new DataSet();
            command.Parameters.Add("@clienteid", SqlDbType.Int).Value = clienteid == null ? 0:clienteid ;
            command.Parameters.Add("@sucursalid", SqlDbType.Int).Value = sucursalid == null ? 0 :sucursalid;
            command.Parameters.Add("@facEstatus", SqlDbType.Int).Value = facEstatus == null ? 10 :facEstatus;
            command.Parameters.Add("@desde", SqlDbType.Date).Value = desde.Date;
            command.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.Date;

           


            command.Connection = connection;
            connection.Open();
                SqlDataAdapter dt = new SqlDataAdapter(command);
                dt.Fill(ds);
            var a = ds.Tables[0].AsEnumerable().Select(r => new FacturaView
            {
                ProductoId = r.Field<int>("ProductoId"),
                ProductName  = r.Field<string>("Producto"),
                FacturaNo = r.Field<string>("FacturaNo"),
                ClienteFecha = r.Field<DateTime>("ClienteFecha"),
                ClienteNombre = r.Field<string>("ClienteNombre"),
                ClienteSucursal = r.Field<string>("ClienteSucursal"),
                ClientePlan = r.Field<string>("ClientePlan"),
                estado = r.Field<string>("estado"),
                precio = r.Field<float>("precio"),
                FacturaId = r.Field<int>("FacturaId")

            });

            return Json(a, JsonRequestBehavior.DenyGet);

        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public JsonResult GetCuadre(int? clienteid, int? sucursalid, DateTime desde, DateTime hasta, int? facEstatus,string UserName)
        {

            string exampleSQL = "select df.descripcion as Producto, f.FacturaId,f.UserName, df.ProductoId,c.Nombre as ClienteNombre,s.Nombre as ClienteSucursal,(convert(varchar, p.planId) + c.ClienteCodigo + convert(varchar, f.FacturaId)) as FacturaNo,p.descripcion as ClientePlan,f.FechaFactura as ClienteFecha,df.precio as precio,(case when FacEstatus = 1 then 'Digitada' else 'Anulada' end) as estado from Facturas f inner join Clientes c on f.clienteid = c.clienteid inner join DetalleFacturas df on f.facturaid = df.FacturaId inner join Plans p on c.planid = p.planid inner join Sucursals s on c.SucursalId = s.SucursalId where (f.UserName like '%' + @UserName  + '%') and (@clienteid = 0   OR f.clienteid = @clienteid ) and (@sucursalid = 0  or c.sucursalid = @sucursalid) and(@facestatus = 10 OR f.FacEstatus = @facestatus) and(cast(f.FechaFactura as date) between @desde and @hasta)";
            SqlConnection connection = new SqlConnection(ConnectionStringDB());
            SqlCommand command = new SqlCommand(exampleSQL, connection);
            DataSet ds = new DataSet();
            command.Parameters.Add("@clienteid", SqlDbType.Int).Value = clienteid == null ? 0 : clienteid;
            command.Parameters.Add("@sucursalid", SqlDbType.Int).Value = sucursalid == null ? 0 : sucursalid;
            command.Parameters.Add("@facEstatus", SqlDbType.Int).Value = facEstatus == null ? 10 : facEstatus;
            command.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName == null ? "" : UserName;
            command.Parameters.Add("@desde", SqlDbType.Date).Value = desde.Date;
            command.Parameters.Add("@hasta", SqlDbType.Date).Value = hasta.Date;




            command.Connection = connection;
            connection.Open();
            SqlDataAdapter dt = new SqlDataAdapter(command);
            dt.Fill(ds);
            var a = ds.Tables[0].AsEnumerable().Select(r => new FacturaView
            {
                ProductoId = r.Field<int>("ProductoId"),
                ProductName = r.Field<string>("Producto"),
                FacturaId = r.Field<int>("FacturaId"),
                FacturaNo = r.Field<string>("FacturaNo"),
                ClienteFecha = r.Field<DateTime>("ClienteFecha"),
                ClienteNombre = r.Field<string>("ClienteNombre"),
                ClienteSucursal = r.Field<string>("ClienteSucursal"),
                ClientePlan = r.Field<string>("ClientePlan"),
                estado = r.Field<string>("estado"),
                precio = r.Field<float>("precio"),
                UserName = r.Field<string>("UserName")

            });

            return Json(a, JsonRequestBehavior.DenyGet);

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

               /* PrinterSettings settings = new PrinterSettings();
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrinterSettings.PrinterName = settings.PrinterName;

               
                printDocument.PrintPage += new PrintPageEventHandler(PrintPage);
                printDocument.Print();
                */

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
           //// return RedirectToAction("NuevaFactura");

        }


        [Authorize(Roles = "View")]
        public void Conduce80MM()
        {

            try
            {
                
                var cabecera = db.Facturas.Max(fac => fac.FacturaId);

                SetFactura(cabecera);
                var facturas = DSFactura.Tables[0].AsEnumerable().Select(r => new FacturaView {
                    FacturaNo = r.Field<string>("Numero"),
                    ClienteFecha = r.Field<DateTime>("FechaFactura"),
                    ClienteNombre = r.Field<string>("Nombre"),
                    ClienteDireccion = r.Field<string>("Direccion"),
                    ClienteTelefono = r.Field<string>("Telefono"),
                    ProductName = r.Field<string>("descripcion"),
                    precio =(float) r.Field<decimal>("precio"),
                    ClientePlan = r.Field<string>("descripcion1"),

                    cantidad = 1,
                    
                });

                var Renderer = new IronPdf.HtmlToPdf();
                var PDF = Renderer.RenderHtmlAsPdf(
    @"<div style = 'text-align:left' >"
        + "<img src = '~/Content/img/hd.PNG' />"
    + "</div>"
   + "<div style = 'text-align:left' >"
        + "<h3> Funeraria Hato Damas </h3>"
        + "<p> Presente en tus Momentos Tristes </p>"
        + "<p> TEL.: 809 - 289 - 8181 / 829 - 521 - 2311 </p>"

           + "<hr />"

           + "<div style = 'text-align:left' >< strong > NoFactura: </ strong >" + facturas.FirstOrDefault().FacturaNo + "</div>"

                  + "<p><strong> Cliente: </strong>" + facturas.FirstOrDefault().ClienteNombre + "</p>"

                       + "<p><strong> Telefono: </strong> " + facturas.FirstOrDefault().ClienteTelefono + " </p>"

                           + "<p><strong> Direccion: </strong >" + facturas.FirstOrDefault().ClienteDireccion + " </p>"

                                + "<p><strong>" + facturas.FirstOrDefault().ClientePlan + "</strong>" + "($" + facturas.FirstOrDefault().precio + ")</p>"

                                      + "    <hr/>"

                                       + "   <h4> Detalle Factura </h4>"


                                           + "  <table style ='text-align:left;font-size:13px;'>"


                                             + "     <thead>"


                                               + "       <tr>"


                                                  + "        < th > Descripcion </ th >"


                                                        + "  < th > Monto </ th >"


                                                + "      </ tr >"


                                                + "  </ thead >"


          //                                + "  < tbody >" +
          //                                      foreach(var item in facturas)
          //{
          //    "< tr >"

          //        < td > @item.ProductName </ td >

          //        < td >$ @item.precio </ td >


          //      </ tr >

          //}
          //</ tbody >

          + " < tfoot >"

               + " < tr >"

                 + "  < th > Total </ th >"

                   + " < th >$" + facturas.Sum(x => x.precio) + "</ th >"


                 + " </ tr >"


              + "</ tfoot >"


         + " </ table >"



         + " < br />"

        + "  < br />"

         + " < br />"


        + "  < div > _________________________________________ </ div >"

         + " < div >< strong > Entregado a: </ strong > </ div >"

            + "   < br />"


   + " </ div >");

                //var pdff = Renderer.RenderHtmlAsPdf(PDF);
                var OutputPath = "Recibo.pdf";
               // PDF.SaveAs(Path.Combine(Request.PhysicalApplicationPath, OutputPath));

                Response.Clear();
                Response.AddHeader("content-disposition", "attachment;filename=" + OutputPath);
                Response.WriteFile(Server.MapPath( "/" + OutputPath));
                Response.End();
                // This neat trick opens our PDF file so we can see the result in our default PDF viewer
                //System.Diagnostics.Process.Start(OutputPath);

                //   return View(facturas);

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message.ToString());
             //   return RedirectToAction("NuevaFactura");
            }
            

        }

        public ActionResult ReporteGeneral(int? clienteId, int? sucursalId, string fechade, string fechaha, int? facestatus,string UserName)
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

                SetFacturaGeneral(clienteId, sucursalId, fechade, fechaha, facestatus,UserName);
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

        private void SetFacturaGeneral(int? clienteId, int? sucursalId, string fechade, string fechaha, int? facestatus,string UserName)
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
                cmd.Parameters.Add("@UserName", SqlDbType.VarChar).Value = UserName;




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

        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            // Se crea un objeto Metafile que define un grafico
            // en base a la información contenida en un stream
            Metafile pageImage = new Metafile(pages[currentPageIndex]);

            // Se dibuja la página en el reporte
            ev.Graphics.DrawImage(pageImage, ev.PageBounds);

            // Se procesan las paginas siguientes
            currentPageIndex++;
            ev.HasMorePages = (currentPageIndex < pages.Count);
        }

        /// <summary>
        /// Export the given report as an EMF (Enhanced Metafile) file.
        /// </summary>
        /// <param name="report"></param>
        private void Export(LocalReport report)
        {
            string deviceInfo =
                "<DeviceInfo>" +
                // "  <OutputFormat>EMF</OutputFormat>" +
                "  <OutputFormat>PDF</OutputFormat>" +
                "  <PageWidth>11in</PageWidth>" +
                "  <PageHeight>6in</PageHeight>" +
                "  <MarginTop>0.25in</MarginTop>" +
                "  <MarginLeft>0.25in</MarginLeft>" +
                "  <MarginRight>0.0in</MarginRight>" +
                "  <MarginBottom>0.0in</MarginBottom>" +
                "</DeviceInfo>";

            Warning[] warnings;

            try
            {
                // El método render es el encargado de crear el stream
                // (llamando a CreateStream) en el formato especificado,
                // usando el stream proveido por la funcion de callback
                // en este caso, un aun archivo EMF con ciertas medidas
                report.Render("Image", deviceInfo, CreateStream, out warnings);

                // Se encarga de resetear la posicion de
                // todos los stream al inicio de los mismos
                foreach (Stream stream in pages)
                    stream.Position = 0;
            }
            catch (LocalProcessingException ex)
            {
                throw ex;
            }
        }

        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            // Creamos un MemoryStream para no tener que grabar en el
            // disco duro cada una de las páginas que pudiera haber
            Stream stream = new MemoryStream();

            // Se añade el stream al listado
            pages.Add(stream);

            return stream;
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