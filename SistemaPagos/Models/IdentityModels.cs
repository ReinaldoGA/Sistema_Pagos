using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace FunerariaProyecto.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("FunerariaProyectoContext", throwIfV1Schema: false)
        {
        }


        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Product> Products { get; set; }

        public System.Data.Entity.DbSet<FunerariaProyecto.Models.DetallePago> DetallePagoes { get; set; }

        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Pago> Pagoes { get; set; }

        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Plan> Plans { get; set; }

        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Cliente> Clientes { get; set; }

        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Tipo> Tipoes { get; set; }
        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Sucursal> Sucursals { get; set; }

        public System.Data.Entity.DbSet<FunerariaProyecto.Models.Facturas> Facturas { get; set; }
        public System.Data.Entity.DbSet<FunerariaProyecto.Models.DetalleFactura> DetalleFacturas { get; set; }
        public System.Data.Entity.DbSet<FunerariaProyecto.Models.ClienteDetalle> ClienteDetalle { get; set; }


        public static ApplicationDbContext Create()
        {


            return new ApplicationDbContext();
        }
    }
}