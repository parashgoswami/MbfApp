using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos.Member;
using Microsoft.AspNetCore.Identity;

namespace MbfApp.Services.MemberServices;

public interface IMemberService
{
    public Task CreateNewMember(MemberRequestDto request);
}

public class MemberService : IMemberService
{
    private AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MemberService(UserManager<ApplicationUser> userManager, AppDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task CreateNewMember(MemberRequestDto request)
    {
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
            LocationId = 1,
        };
        _context.Members.Add(member);
        await _context.SaveChangesAsync();
        var appUser = new ApplicationUser
        {
            UserName = member.Email,
            Email = member.Email,
            DisplayName = $"{member.FirstName} {member.LastName}",
            MemberId = member.Id,
            PhoneNumber = member.Phone
        };

        var result = await _userManager.CreateAsync(appUser, "Welcome@123");
        if (!result.Succeeded)
        {
            // Rollback the member if needed
            _context.Members.Remove(member);
            await _context.SaveChangesAsync();            
        }

    }
}
