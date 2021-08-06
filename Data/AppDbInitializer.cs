using Hotvenues.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotvenues.Data
{
    public class AppDbInitializer
    {
        public static async Task InitializeAsync(AppDbContext context, IServiceProvider serviceProvider)
        {
            //NB: "EnsureCreated()" bypasses Migrations to create the schema, which causes "Migrate()" to fail
            //context.Database.EnsureCreated(); 
            context.Database.Migrate(); //Auto Migrations

            //Creating Roles
            await SeedRoles(serviceProvider.GetRequiredService<RoleManager<IdentityRole>>());

            //Create Default Admin User
            await SeedUsers(serviceProvider.GetRequiredService<UserManager<User>>());

            //Create App Settings
            SeedAppSettings(context);

            context.SaveChanges();
        }


        private static void SeedAppSettings(AppDbContext context)
        {
            var appSettings = new List<AppSetting>
            {
                new AppSetting {Name = ConfigKeys.SenderName, Value = "TuffClinic"},
                new AppSetting {Name = ConfigKeys.ApiKey, Value = "NONE"}
            };

            foreach (var setting in appSettings.Where(p => !context.AppSettings.Any(x => x.Name == p.Name)))
            {
                context.AppSettings.Add(setting);
            }

        }

        private static async Task SeedUsers(UserManager<User> userManager)
        {
            var res = await userManager.CreateAsync(new User
            {
                UserName = "Admin",
                PhoneNumber = "0000000000",
                Name = "Administrator",
                Email = "info@app.com",
                Profile = new Profile
                {
                    Name = "Administrator",
                    Description = "Administrator Role",
                    Privileges = RoleNames.Aggregate((a, b) => $"{a},{b}"),
                    Locked = true
                },
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }, "@dm1n@pp");

            if (res.Succeeded)
            {
                var user = userManager.FindByNameAsync("Admin").Result;
                await userManager.AddToRolesAsync(user, RoleNames);
            }
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var roleName in RoleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist) await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static readonly string[] RoleNames = {
            Privileges.Administration,
            Privileges.Setting,
            Privileges.Report,
            Privileges.Dashboard,
            Privileges.MessagePortal,

            Privileges.UserCreate,
            Privileges.UserUpdate,
            Privileges.UserRead,
            Privileges.UserDelete,

            Privileges.RoleCreate,
            Privileges.RoleUpdate,
            Privileges.RoleRead,
            Privileges.RoleDelete
        };
    }
}
