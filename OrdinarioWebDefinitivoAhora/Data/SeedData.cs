using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrdinarioWebDefinitivoAhora.Models;

namespace OrdinarioWebDefinitivoAhora.Data
{
    public static class SeedData
    {
        public static void EnsureSeeded(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.Migrate();


            // SEMILLA DE USUARIOS
 
            if (!context.Users.Any())
            {
                var hasher = new PasswordHasher<UserModel>();

                var users = new List<UserModel>
                {
                    new UserModel { NumEmpleado = "0001", Nombre = "Admin", Role = "Admin" },
                    new UserModel { NumEmpleado = "0002", Nombre = "Usuario A", Role = "User" },
                    new UserModel { NumEmpleado = "0003", Nombre = "Usuario B", Role = "User" },
                    new UserModel { NumEmpleado = "0004", Nombre = "Usuario C", Role = "User" },
                    new UserModel { NumEmpleado = "0005", Nombre = "Usuario D", Role = "User" },
                };

                foreach (var u in users)
                {
                    u.PasswordHash = hasher.HashPassword(u, "Pass123!");
                    context.Users.Add(u);
                }

                context.SaveChanges();
            }


            // SEMILLA DE FOLIOS (500 folios del año actual)

            int currentYear = DateTime.Now.Year;

            if (!context.Folios.Any(f => f.Year == currentYear))
            {
                var folios = new List<FolioModel>();

                for (int i = 1; i <= 500; i++)
                {
                    folios.Add(new FolioModel
                    {
                        Year = currentYear,
                        Number = i,
                        Estado = FolioEstado.Disponible
                    });
                }

                context.Folios.AddRange(folios);
                context.SaveChanges();
            }
        }
    }
}
