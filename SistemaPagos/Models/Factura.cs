using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Facturas
    {
        [Key]
        public int FacturaId { get; set; }
        public DateTime FechaFactura { get; set; }
        public int ClienteId { get; set; } 
        public int FacEstatus { get; set; }
        public virtual Cliente Cliente { get; set; }
        public virtual ICollection<DetalleFactura> DetalleFacturas { get; set; }

    }
}