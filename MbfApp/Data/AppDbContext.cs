using MbfApp.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MbfApp.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Location> Locations { get; set; }
    public DbSet<VoucherSequence> VoucherSequences { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<FinYear> FinYears { get; set; }
    public DbSet<Account> AccountBalances { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Journal> Journals { get; set; }
    public DbSet<MemberLedger> MemberLedgers { get; set; }
    public DbSet<MemberLedgerBalance> MemberLedgerBalances { get; set; }
    public DbSet<Loan> Loans { get; set; }
    public DbSet<Withdrawal> Withdrawals { get; set; }
    
//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         // Enforce unique constraint
//         modelBuilder.Entity<MemberLedger>()
//             .HasIndex(m => new { m.EmpCode, m.YearMonth })
//             .IsUnique();
//     }
}
