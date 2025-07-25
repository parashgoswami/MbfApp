using MbfApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MbfApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{  
    public DbSet<Member> Members { get; set; } = default!;
    public DbSet<Location> Locations { get; set; } = default!;
    public DbSet<AccountCode> AccountCodes { get; set; } = default!;
    public DbSet<Loan> Loans { get; set; } = default!;
    public DbSet<Withdrawal> Withdrawals { get; set; } = default!;
}
