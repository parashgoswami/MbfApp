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
                        StartDate = new DateTime(2025, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                        EndDate = new DateTime(2026, 3, 31, 0, 0, 0, DateTimeKind.Utc),
                        IntDeposit = 7.50f,
                        IntLoan = 8.50f,
                    },
                    new FinYear
                    {
                        FinYrLabel = "2024-2025",
                        StartDate = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                        EndDate = new DateTime(2025, 3, 31, 0, 0, 0, DateTimeKind.Utc),
                        IntDeposit = 7.50f,
                        IntLoan = 8.50f,
                        IsClosed = true,
                        IsCurrent = false
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

    public static async Task SeedMembersAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!await context.Members.AnyAsync())
        {
            Console.WriteLine("Seeding Members...");

            var members = new List<Member>
                {
                    new Member { EmployeeNo = "1001", FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "1234567890", JoiningDate = new DateTime(2020, 1, 15, 0, 0, 0, DateTimeKind.Utc), LocationId = 1, Share = 100, Nominee = "Jane Doe" },
                    new Member { EmployeeNo = "1002", FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "2345678901", JoiningDate = new DateTime(2019, 3, 22, 0, 0, 0, DateTimeKind.Utc), LocationId = 2, Share = 150, Nominee = "John Smith" },
                    new Member { EmployeeNo = "1003", FirstName = "Peter", LastName = "Jones", Email = "peter.jones@example.com", Phone = "3456789012", JoiningDate = new DateTime(2021, 5, 30, 0, 0, 0, DateTimeKind.Utc), LocationId = 3, Share = 200, Nominee = "Mary Jones" },
                    new Member { EmployeeNo = "1004", FirstName = "Mary", LastName = "Williams", Email = "mary.williams@example.com", Phone = "4567890123", JoiningDate = new DateTime(2018, 7, 10, 0, 0, 0, DateTimeKind.Utc), LocationId = 4, Share = 250, Nominee = "David Williams" },
                    new Member { EmployeeNo = "1005", FirstName = "David", LastName = "Brown", Email = "david.brown@example.com", Phone = "5678901234", JoiningDate = new DateTime(2022, 9, 5, 0, 0, 0, DateTimeKind.Utc), LocationId = 5, Share = 300, Nominee = "Susan Brown" },
                    new Member { EmployeeNo = "1006", FirstName = "Susan", LastName = "Davis", Email = "susan.davis@example.com", Phone = "6789012345", JoiningDate = new DateTime(2017, 11, 12, 0, 0, 0, DateTimeKind.Utc), LocationId = 6, Share = 350, Nominee = "Michael Davis" },
                    new Member { EmployeeNo = "1007", FirstName = "Michael", LastName = "Miller", Email = "michael.miller@example.com", Phone = "7890123456", JoiningDate = new DateTime(2023, 1, 20, 0, 0, 0, DateTimeKind.Utc), LocationId = 7, Share = 400, Nominee = "Linda Miller" },
                    new Member { EmployeeNo = "1008", FirstName = "Linda", LastName = "Wilson", Email = "linda.wilson@example.com", Phone = "8901234567", JoiningDate = new DateTime(2016, 2, 28, 0, 0, 0, DateTimeKind.Utc), LocationId = 1, Share = 450, Nominee = "Robert Wilson" },
                    new Member { EmployeeNo = "1009", FirstName = "Robert", LastName = "Moore", Email = "robert.moore@example.com", Phone = "9012345678", JoiningDate = new DateTime(2024, 4, 1, 0, 0, 0, DateTimeKind.Utc), LocationId = 2, Share = 500, Nominee = "Patricia Moore" },
                    new Member { EmployeeNo = "1010", FirstName = "Patricia", LastName = "Taylor", Email = "patricia.taylor@example.com", Phone = "0123456789", JoiningDate = new DateTime(2015, 6, 18, 0, 0, 0, DateTimeKind.Utc), LocationId = 3, Share = 550, Nominee = "James Taylor" },
                    new Member { EmployeeNo = "1011", FirstName = "James", LastName = "Anderson", Email = "james.anderson@example.com", Phone = "1122334455", JoiningDate = new DateTime(2020, 8, 25, 0, 0, 0, DateTimeKind.Utc), LocationId = 4, Share = 600, Nominee = "Barbara Anderson" },
                    new Member { EmployeeNo = "1012", FirstName = "Barbara", LastName = "Thomas", Email = "barbara.thomas@example.com", Phone = "2233445566", JoiningDate = new DateTime(2019, 10, 3, 0, 0, 0, DateTimeKind.Utc), LocationId = 5, Share = 650, Nominee = "Thomas Thomas" },
                    new Member { EmployeeNo = "1013", FirstName = "Thomas", LastName = "Jackson", Email = "thomas.jackson@example.com", Phone = "3344556677", JoiningDate = new DateTime(2021, 12, 9, 0, 0, 0, DateTimeKind.Utc), LocationId = 6, Share = 700, Nominee = "Jessica Jackson" },
                    new Member { EmployeeNo = "1014", FirstName = "Jessica", LastName = "White", Email = "jessica.white@example.com", Phone = "4455667788", JoiningDate = new DateTime(2018, 2, 14, 0, 0, 0, DateTimeKind.Utc), LocationId = 7, Share = 750, Nominee = "Daniel White" },
                    new Member { EmployeeNo = "1015", FirstName = "Daniel", LastName = "Harris", Email = "daniel.harris@example.com", Phone = "5566778899", JoiningDate = new DateTime(2022, 4, 21, 0, 0, 0, DateTimeKind.Utc), LocationId = 1, Share = 800, Nominee = "Nancy Harris" }
                };

            await context.Members.AddRangeAsync(members);
            await context.SaveChangesAsync();
            Console.WriteLine("Member data seeded successfully.");
        }
        else
        {
            Console.WriteLine("Member data already exists. Skipping seeding.");

        }
    }
}

