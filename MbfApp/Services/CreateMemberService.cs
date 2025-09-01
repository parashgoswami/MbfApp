using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.Member;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services;
public interface ICreateMemberService
{
    public Task CreateMember(MemberRequestDto request);
}
public class CreateMemberService(UserManager<ApplicationUser> userManager, AppDbContext context, IFinYearService finYearService) : ICreateMemberService
{
    public async Task CreateMember(MemberRequestDto request)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            var empNoExists = await context.Set<Member>()
            .AnyAsync(a => a.EmployeeNo == request.EmployeeNo);

            if (empNoExists)
                throw new InvalidOperationException("Employee number must be unique.");

            var member = new Member
            {
                EmployeeNo = request.EmployeeNo,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                Share = request.Share,
                Nominee = request.Nominee,
                JoiningDate = request.JoiningDate,
                LocationId = request.LocationId
            };
            context.Members.Add(member);
            await context.SaveChangesAsync();
            var finYear = await finYearService.getCurrentFinYear();
            if (finYear != null)
            {
                var ledgerBalance = new MemberLedgerBalance
                {
                    MemberId = member.Id,
                    FinYearId = finYear.Id,
                    OpBalDeposit = 0,
                    OpBalLoan = 0,
                    DepositBalance = 0,
                    LoanBalance = 0
                };

                context.MemberLedgerBalances.Add(ledgerBalance);
            }
            await context.SaveChangesAsync();

            var appUser = new ApplicationUser
            {
                UserName = member.Email,
                Email = member.Email,
                DisplayName = $"{member.FirstName} {member.LastName}",
                MemberId = member.Id,
                PhoneNumber = member.Phone
            };
            var result = await userManager.CreateAsync(appUser, "Welcome@123");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(appUser, "Member");
            }
            else
            {
                throw new InvalidOperationException(
                    "User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description))
                );
            }
            await transaction.CommitAsync();
        }
        catch
        {
            // ❌ Rollback on any failure
            await transaction.RollbackAsync();
            throw;
        }
    }

}
