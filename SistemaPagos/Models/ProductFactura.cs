using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class ProductFactura : Product
    {
        public float cantidad { get; set; }
        public decimal valor { get { return precioo * (decimal)cantidad; } }
        public virtual List<Cliente> cliente { get; set; }
        public int ClienteId { get; set; }
        public int PlanId { get; set; }
        public decimal precioo { get; set; }
       
    }
}