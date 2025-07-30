using Microsoft.AspNetCore.Identity;

namespace MbfApp.Data;

public static class AppDbSeed
{
    public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminEmail = "admin@localhost.com";
        const string adminPassword = "Admin@123"; // Change as needed for production
        const string adminRole = "Administrator";

        const string memberRole = "Member";

        // Ensure role exists
        if (!await roleManager.RoleExistsAsync(memberRole))
        {
            await roleManager.CreateAsync(new IdentityRole(memberRole));
        }
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }
        // Ensure user exists
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                DisplayName = "Administrator",
                MemberId = 0 
            };
            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        
        }
        else
        {
            // Ensure user is in role
            if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            {
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}
