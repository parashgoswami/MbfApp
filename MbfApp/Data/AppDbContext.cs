using Microsoft.EntityFrameworkCore;
using MbfApp.Data.Entities;

namespace MbfApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> Members { get; set; } = default!;
    public DbSet<Location> Locations { get; set; } = default!;
    public DbSet<AccountCode> AccountCodes { get; set; } = default!;
    public DbSet<Loan> Loans { get; set; } = default!;
    public DbSet<Withdrawal> Withdrawals { get; set; } = default!;
}
