using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Tipo
    {
        public int TipoId { get; set; }
        public string Descripcion { get; set; }
        public virtual ICollection<Pago> pagos { get; set; }


    }
}