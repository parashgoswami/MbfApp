using MbfApp.Data.Entities;
using MbfApp.Data.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public static async Task SeedAccountAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is created and migrations are applied            
            await context.Database.MigrateAsync();

            if (!await context.Accounts.AnyAsync())
            {

                Console.WriteLine("Seeding chart of Accounts...");

                var accounts = new List<Account>
                {
                    new Account
                    {
                        AccountLabel = "Bank A/c",
                        AccountType = AccountType.Asset
                    },
                    new Account
                    {
                        AccountType = AccountType.Asset,
                        AccountLabel = "Cash A/c"
                    } ,
                    new Account
                    {
                        AccountType = AccountType.Asset,
                        AccountLabel = "Loan A/c"
                    } ,
                    new Account
                    {
                        AccountType = AccountType.Liabilities,
                        AccountLabel = "Deposit A/c"
                    } ,
                    new Account
                    {
                        AccountType = AccountType.Liabilities,
                        AccountLabel = "Int. On Deposit A/c"
                    } ,
                    new Account
                    {
                        AccountType = AccountType.Income,
                        AccountLabel = "Int. On Loan A/c"
                    } ,

                };
                await context.Accounts.AddRangeAsync(accounts);
                await context.SaveChangesAsync();
                Console.WriteLine("Chart of Accounts seeded successfully.");
            }
            else
            {
                Console.WriteLine("Chart of Accounts already exists. Skipping seeding.");
            }
        }
    }
    public static async Task SeedLocationAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is created and migrations are applied            
            await context.Database.MigrateAsync();

            if (!await context.Locations.AnyAsync())
            {

                Console.WriteLine("Seeding Locations...");

                var locations = new List<Location>
                {
                    new Location
                    {
                        Name="AGBPS"                        
                    },
                    new Location
                    {
                        Name = "SHILLONG"
                    } ,
                    new Location
                    {
                        Name = "GUWAHATI"
                    } ,
                    new Location
                    {
                        Name = "KHPS"
                    } ,
                    new Location
                    {
                        Name = "PLHPS"
                    } ,
                    new Location
                    {
                        Name = "PHPS"
                    } ,
                    new Location
                    {
                        Name = "DHPS"
                    }
                };
                await context.Locations.AddRangeAsync(locations);
                await context.SaveChangesAsync();
                Console.WriteLine("Location data seeded successfully.");
            }
            else
            {
                Console.WriteLine("Location data already exists. Skipping seeding.");
            }
        }
    }
    public static async Task SeedFinYearAsync(IServiceProvider serviceProvider)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure the database is created and migrations are applied            
            await context.Database.MigrateAsync();

            if (!await context.FinYears.AnyAsync())
            {
                
                Console.WriteLine("Seeding FinYear data...");

                var finYears = new List<FinYear>
                {
                    new FinYear
                    {
                        
                        FinYrLabel = "2025-2026",
                        StartDate = new DateTime(2025, 4, 1),
                        EndDate = new DateTime(2026, 3, 31),
                        IntDeposit = 7.50f,
                        IntLoan = 8.50f,                        
                    },
                    new FinYear
                    {
                        FinYrLabel = "2024-2025",
                        StartDate = new DateTime(2024, 4, 1),
                        EndDate = new DateTime(2025, 3, 31),
                        IntDeposit = 7.50f,
                        IntLoan = 8.50f,
                        IsClosed = true 
                    },
                    
                };
                await context.FinYears.AddRangeAsync(finYears);
                await context.SaveChangesAsync();
                Console.WriteLine("FinYear data seeded successfully.");
            }
            else
            {
                Console.WriteLine("FinYear data already exists. Skipping seeding.");
            }
        }
    }
}

