using FunerariaProyecto.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SistemaPagos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace SistemaPagos
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {

            ApplicationDbContext db = new ApplicationDbContext();
            //ApplicationDbContext db = new ApplicationDbContext();
            CreateRoles(db);
            CreateSuperUser(db);
            AddPermisionToSuperUesr(db);
            db.Dispose();
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        private void AddPermisionToSuperUesr(ApplicationDbContext db)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            var user = userManager.FindByName("omegajuan2014@hotmail.com");
            if (!userManager.IsInRole(user.Id, "View"))
            {
                userManager.AddToRole(user.Id, "View");
            }

            if (!userManager.IsInRole(user.Id, "Edit"))
            {
                userManager.AddToRole(user.Id, "Edit");
            }

            if (!userManager.IsInRole(user.Id, "Detail"))
            {
                userManager.AddToRole(user.Id, "Detail");
            }

            //if (!userManager.IsInRole(user.Id, "Delete"))
            //{
            //    userManager.AddToRole(user.Id, "Delete");
            //}

            if (!userManager.IsInRole(user.Id, "Create"))
            {
                userManager.AddToRole(user.Id, "Create");
            }
            if (!userManager.IsInRole(user.Id, "Admin"))
            {
                userManager.AddToRole(user.Id, "Admin");
            }
            if (!userManager.IsInRole(user.Id, "User"))
            {
                userManager.AddToRole(user.Id, "User");
            }
            //if (!userManager.IsInRole(user.Id, "Mantenimientos"))
            //{
            //    userManager.AddToRole(user.Id, "Mantenimientos");
            //}
        }

        private void CreateSuperUser(ApplicationDbContext db)
        {

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            var user = userManager.FindByName("omegajuan2014@hotmail.com");
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = "omegajuan2014@hotmail.com",
                    Email = "omegajuan2014@hotmail.com"
                };

                userManager.Create(user, "Omega114785.");
            }
        }

        private void CreateRoles(ApplicationDbContext db)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            if (!roleManager.RoleExists("View"))
            {
                roleManager.Create(new IdentityRole("View"));
            }
            if (!roleManager.RoleExists("Edit"))
            {
                roleManager.Create(new IdentityRole("Edit"));
            }
            if (!roleManager.RoleExists("Create"))
            {
                roleManager.Create(new IdentityRole("Create"));
            }
            if (!roleManager.RoleExists("Detail"))
            {
                roleManager.Create(new IdentityRole("Detail"));
            }
            if (!roleManager.RoleExists("OrdenCompra"))
            {
                roleManager.Create(new IdentityRole("OrdenCompra"));
            }
            if (!roleManager.RoleExists("Facturacion"))
            {
                roleManager.Create(new IdentityRole("Facturacion"));
            }
            if (!roleManager.RoleExists("Inventario"))
            {
                roleManager.Create(new IdentityRole("Inventario"));
            }
            if (!roleManager.RoleExists("Consulta"))
            {
                roleManager.Create(new IdentityRole("Consulta"));
            }
            if (!roleManager.RoleExists("Clientes"))
            {
                roleManager.Create(new IdentityRole("Clientes"));
            }
            if (!roleManager.RoleExists("Admin"))
            {
                roleManager.Create(new IdentityRole("Admin"));
            }
            if (!roleManager.RoleExists("User"))
            {
                roleManager.Create(new IdentityRole("User"));
            }
            if (!roleManager.RoleExists("Mantenimiento"))
            {
                roleManager.Create(new IdentityRole("Mantenimiento"));
            }
        }

    }
    }

