using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SistemaPagos.Models
{
    public class ClientesViewModel
    {
        public int ClienteId { get; set; }
        public string ClienteCodigo { get; set; }
        public string Nombre { get; set; }
        public string Sucursal { get; set; }
    }
}