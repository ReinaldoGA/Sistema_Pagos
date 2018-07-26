using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Plan
    {
        [Key]
        public int PlanId { get; set; }
        [Display(Name ="Planes")]
        public string descripcion { get; set; }
        public decimal precio { get; set; }
        public virtual ICollection<Cliente> clientes { get; set; }

    }
}