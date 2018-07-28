using FunerariaProyecto.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.ViewModels
{
    public class FacturaView
    {
        public float cantidad;
        public float precio;
        public string estado;

        public decimal Monto { get { return (decimal)cantidad * (decimal)precio; } }
        public Cliente Cliente { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteSucursal { get; set; }
        public string ClientePlan { get; set; }
        public int ClienteId { get; set; }
        public int ClienteId2 { get; set; }

        public int ProductoId { get; set; }
        public int FacturaId { get; set; }
        public ProductFactura Products { get; set; }
        public List<ProductFactura> Product { get; set; }
        public string ClienteCodigo { get; internal set; }
        public string ClienteCedula { get; internal set; }
        public string ClienteDireccion { get; internal set; }
        public string ClienteTelefono { get; internal set; }
        public DateTime ClienteFecha { get; internal set; }
        public string ProductName { get; internal set; }
        public string UserName { get; internal set; }
    }
}