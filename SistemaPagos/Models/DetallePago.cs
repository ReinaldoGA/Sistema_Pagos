using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class DetallePago
    {
        [Key]
        public int DetallePagoId { get; set; }
        public int PagoId { get; set; }
        public int productoId { get; set; }
        public int facturaId { get; set; }
        public int DetalleFacturaId { get; set; }
        public string descripcion { get; set; }
        public float cantidad { get; set; }
        public decimal precio { get; set; }
        public virtual Pago pago { get; set; }
        public virtual Product products { get; set; }


    }
    }