using ControlStock.Data;
using ControlStock.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ControlStock.Areas.Infrastructure
{
    public class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<MyDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<MyUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Aplicar migraciones
            context.Database.Migrate();

            // Crear rol administrador si no existe
            string adminRole = "Administrador";
            string userRole = "Usuario";
            string coordRole= "Coordinador";
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
                await roleManager.CreateAsync(new IdentityRole(userRole));
                await roleManager.CreateAsync(new IdentityRole(coordRole));
            }

            // Crear usuario administrador si no existe
            string adminUserName = "admin";
            string adminPassword = "Admin123456";

            var adminUser = await userManager.FindByNameAsync(adminUserName);
            if (adminUser == null)
            {
                var user = new MyUser
                {
                    UserName = adminUserName,
                    Email = "admin@local",
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRole);
                }           
                // Agregar otros datos necesarios (por ejemplo Scopes y Sections)
                if (!await context.Scopes.AnyAsync() || !await context.Sections.AnyAsync())
                {
                    context.Scopes.Add(new Scope { ScopeName  = "Todos", CreatedDate = DateTime.Now, EliminadoLogico = false, UserId = user.Id });                
                    context.Sections.Add(new Section { Name = "Todos", Abreviatura="Todos" });
                    await context.SaveChangesAsync();
                    context.UserPermissions.Add(new UserPermission { UserId = user.Id, ScopeId =1, SectionId = 1 });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
