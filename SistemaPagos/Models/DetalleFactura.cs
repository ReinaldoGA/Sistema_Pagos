using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class DetalleFactura
    {
        [Key]
        public int DetalleFacturaId { get; set; }
        public int FacturaId { get; set; }
        public int productoId { get; set; }
        public string descripcion { get; set; }
        public float precio { get; set; }
        public float cantidad { get; set; }
        public virtual Facturas Factura { get; set; }
        public virtual Product Product { get; set; }

    }
}