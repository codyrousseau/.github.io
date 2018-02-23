namespace crousseau2_College_Strike.DAL.SecurityMigrations
{
    using crousseau2_College_Strike.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<crousseau2_College_Strike.DAL.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DAL\SecurityMigrations";
        }

        protected override void Seed(crousseau2_College_Strike.DAL.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new
                                          RoleStore<IdentityRole>(context));

            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                var roleresult = roleManager.Create(new IdentityRole("Admin"));
            }
            //Create Role Supervisor if it does not exist
            if (!context.Roles.Any(r => r.Name == "Steward"))
            {
                var roleresult = roleManager.Create(new IdentityRole("Steward"));
            }

            var manager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(context));

            var adminuser = new ApplicationUser
            {
                UserName = "admin@outlook.com",
                Email = "admin@outlook.com"
            };

            if (!context.Users.Any(u => u.UserName == "admin@outlook.com"))
            {
                manager.Create(adminuser, "password");
                manager.AddToRole(adminuser.Id, "Admin");
            }

            var stewarduser = new ApplicationUser
            {
                UserName = "steward@outlook.com",
                Email = "steward@outlook.com"
            };

            if (!context.Users.Any(u => u.UserName == "steward@outlook.com"))
            {
                manager.Create(stewarduser, "password");
                manager.AddToRole(stewarduser.Id, "Steward");
            }

            for (int i = 1; i <= 4; i++)
            {
                var user = new ApplicationUser
                {
                    UserName = string.Format("user{0}@outlook.com", i.ToString()),
                    Email = string.Format("user{0}@outlook.com", i.ToString())
                };
                if (!context.Users.Any(u => u.UserName == user.UserName))
                    manager.Create(user, "password");
            }
        }
    }
}
