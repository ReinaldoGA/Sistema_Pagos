using FunerariaProyecto.Models;
using FunerariaProyecto.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SistemaPagos.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FunerariaProyecto.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Users
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var users = userManager.Users.ToList();
            var usersView = new List<UserView>();
            foreach (var user in users)
            {
                var userView = new UserView
                {
                    Email = user.Email,
                    Name = user.UserName,
                    UserId = user.Id
                };
                usersView.Add(userView);
            }
            return View(usersView);
        }
        //[Authorize(Roles = "View")]
        [Authorize(Roles = "Admin")]
        public ActionResult Roles(string userID)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roles = roleManager.Roles.ToList();
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userID);

            var rolesView = new List<RoleView>();
            if (user.Roles != null)
            {
                foreach (var item in user.Roles)
                {
                    var role = roles.Find(r => r.Id == item.RoleId);

                    var roleView = new RoleView
                    {
                        RoleID = role.Id,
                        Name = role.Name
                    };
                    rolesView.Add(roleView);
                }
            }

            var userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserId = user.Id,
                Roles = rolesView

            };

            return View(userView);
        }
        //[Authorize(Roles = "View")]
        [Authorize(Roles = "Admin")]
        public ActionResult AddRoles(string userID)
        {
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roles = roleManager.Roles.ToList();
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userID);

            var rolesView = new List<RoleView>();

            var userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserId = user.Id,
                Roles = rolesView

            };
            //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var list = roleManager.Roles.ToList();
            list.Add(new IdentityRole { Id = "", Name = "[Seleccione un tipo de Rol...]" });
            list = list.OrderBy(r => r.Name).ToList();
            ViewBag.RoleID = new SelectList(list, "Id", "Name");

            return View(userView);

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult AddRoles(string userID, FormCollection form)
        {
            var roleId = Request["RoleId"];

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            var roles = roleManager.Roles.ToList();
            var users = userManager.Users.ToList();
            var user = users.Find(u => u.Id == userID);

            var rolesView = new List<RoleView>();

            var userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserId = user.Id,
                Roles = rolesView

            };
            if (string.IsNullOrEmpty(roleId))
            {
                ViewBag.Error = "Tu debes seleccionar  un rol";

                //var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
                var list = roleManager.Roles.ToList();
                list.Add(new IdentityRole { Id = "", Name = "[Seleccione un tipo de Rol...]" });
                list = list.OrderBy(r => r.Name).ToList();
                ViewBag.RoleID = new SelectList(list, "Id", "Name");

                return View(userView);
            }

            var role = roleManager.Roles.ToList().Find(r => r.Id == roleId);
            if (!userManager.IsInRole(userID, role.Name))
            {
                userManager.AddToRole(userID, role.Name);
            }

            rolesView = new List<RoleView>();

            if (user.Roles != null)
            {
                foreach (var item in user.Roles)
                {
                    role = roles.Find(r => r.Id == item.RoleId);

                    var roleView = new RoleView
                    {
                        RoleID = role.Id,
                        Name = role.Name
                    };
                    rolesView.Add(roleView);
                }
            }

            userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserId = user.Id,
                Roles = rolesView

            };


            return View("Roles", userView);
        }
        public ActionResult Delete(string userID, string roleId)
        {
            if (string.IsNullOrEmpty(userID))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));

            var user = userManager.Users.ToList().Find(u => u.Id == userID);
            var role = roleManager.Roles.ToList().Find(r => r.Id == roleId);

            if (userManager.IsInRole(user.Id, role.Name))
            {
                userManager.RemoveFromRole(user.Id, role.Name);
            }

            var users = userManager.Users.ToList();
            var roles = roleManager.Roles.ToList();
            var rolesView = new List<RoleView>();

            foreach (var item in user.Roles)
            {
                role = roles.Find(r => r.Id == item.RoleId);

                var roleView = new RoleView
                {
                    RoleID = role.Id,
                    Name = role.Name
                };
                rolesView.Add(roleView);
            }


            var userView = new UserView
            {
                Email = user.Email,
                Name = user.UserName,
                UserId = user.Id,
                Roles = rolesView

            };

            return View("Roles", userView);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}