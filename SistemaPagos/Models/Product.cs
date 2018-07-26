using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Product
    {
         
        [Key]
        public int productoId { get; set; }
        public string descripcion { get; set; }
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal precio { get; set; }
        public DateTime fecha { get; set; }
        public float stock { get; set; }
        public virtual ICollection<DetallePago> detallepago { get; set; }
        public virtual ICollection<DetalleFactura> detalleFactuta { get; set; }

    }
}
