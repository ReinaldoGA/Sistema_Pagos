using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Sucursal
    {
        [Key]
        [Display(Name = "Sucrsal")]
        public int SucursalId { get; set; }
        [Display(Name = "Sucursal")]
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public virtual ICollection<Cliente> clientes { get; set; }

    }
}