using FunerariaProyecto.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class ClienteDetalle
    {
        [Key]
        public int detalleClienteID { get; set; }
        public int ClienteID { get; set; }
        public string Nombre { get; set; }
        public string Parentezco { get; set; }
        public string OtrosDatos { get; set; }
      
        public virtual Cliente Cliente { get; set; }
    }
}