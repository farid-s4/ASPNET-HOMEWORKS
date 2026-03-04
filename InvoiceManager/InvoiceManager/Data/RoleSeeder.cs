using Microsoft.AspNetCore.Identity;

namespace InvoiceManager.Data;

public static class RoleSeeder
{
    public static async Task SeedRoles(this IApplicationBuilder app, params string[] roles)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();
            
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}