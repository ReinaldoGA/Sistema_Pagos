using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Pago
    {
        [Key]
        public int PagoId { get; set; }
        public int FacturaId { get; set; }
        public int ClienteId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "fecha pago")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public string Comentario { get; set; }
        public virtual Cliente cliente { get; set; }
        public virtual ICollection<DetallePago> Detallepago { get; set; }


    }
}