//using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Data.Entity.ModelConfiguration.Conventions;
//using System.Linq;
//using System.Web;

//namespace FunerariaProyecto.Models
//{
//    public class ApplicationDbContext : DbContext
//    {
//        // You can add custom code to this file. Changes will not be overwritten.
//        // 
//        // If you want Entity Framework to drop and regenerate your database
//        // automatically whenever you change your model schema, please use data migrations.
//        // For more information refer to the documentation:
//        // http://msdn.microsoft.com/en-us/data/jj591621.aspx
    
//        public ApplicationDbContext() : base("name=ApplicationDbContext")
//        {
//        }
//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
//        }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Product> Products { get; set; }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.DetallePago> DetallePagoes { get; set; }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Pago> Pagoes { get; set; }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Plan> Plans { get; set; }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Cliente> Clientes { get; set; }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Tipo> Tipoes { get; set; }
//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Sucursal> Sucursals { get; set; }

//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Facturas> Facturas { get; set; }
//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.DetalleFactura> DetalleFacturas { get; set; }
//        public System.Data.Entity.DbSet<FunerariaProyecto.Models.ClienteDetalle> ClienteDetalle { get; set; }


        

//    }
//}
