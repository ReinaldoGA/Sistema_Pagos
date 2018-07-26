using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FunerariaProyecto.Models
{
    public class Cliente
    {
        [Key]
        [Display(Name = "Carnet")]
        public int ClienteId { get; set; }
        [Required]
        [Editable(false)]
        public string ClienteCodigo { get; set; }
        [Display(Name = "Sucursal")]
        public int SucursalId { get; set; }
        public int PlanId { get; set; }
        public string Nombre { get; set; }
        
        public string Cedula { get; set; }
        public string Direccion { get; set; }
        [Phone]
        public string Telefono { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Fecha de Inscripcion")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }
        public virtual Sucursal sucursal { get; set; }
        public virtual Plan plan { get; set; }
        public virtual ICollection<Pago> pago { get; set; }
        public virtual ICollection<Facturas> Factura { get; set; }


    }
}