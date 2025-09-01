using MbfApp.Data;
using MbfApp.Data.Entities;
using MbfApp.Dtos;
using MbfApp.Dtos.Member;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MbfApp.Services;

public class MemberService(UserManager<ApplicationUser> userManager, AppDbContext context) : IMemberService
{
    public async Task CreateMember(MemberRequestDto request)
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
        if (!result.Succeeded)
        {
            // Rollback the member if needed
            context.Members.Remove(member);
            await context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedList<MemberDto>> GetPaginatedListAync(MemberSearchParameters searchParams)
    {
        var query = context.Members
            .Include(m => m.Location)
            .Select(m => new MemberDto
            {
                Id = m.Id,
                EmployeeNo = m.EmployeeNo,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                Phone = m.Phone,
                JoiningDate = m.JoiningDate,
                IsActive = m.IsActive,
                Share = m.Share,
                Nominee = m.Nominee,
                LocationId = m.LocationId,
                LocationName = m.Location!.Name
            });

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            var searchTerm = searchParams.SearchTerm.ToLower();
            query = query.Where(m =>
                m.FirstName.ToLower().Contains(searchTerm) ||
                m.LastName.ToLower().Contains(searchTerm) ||
                m.EmployeeNo.ToLower().Contains(searchTerm));
        }
        query = query.OrderBy(m => m.Id);
        return await PaginatedList<MemberDto>.CreateAsync(query.AsNoTracking(), searchParams.PageIndex, searchParams.PageSize);
    }

    public async Task<MemberDto?> GetMemberByIdAsync(int id)
    {
        return await context.Members
            .Include(m => m.Location)
            .Where(m => m.Id == id)
            .Select(m => new MemberDto
            {
                Id = m.Id,
                EmployeeNo = m.EmployeeNo,
                FirstName = m.FirstName,
                LastName = m.LastName,
                Email = m.Email,
                Phone = m.Phone,
                JoiningDate = m.JoiningDate,
                IsActive = m.IsActive,
                Share = m.Share,
                Nominee = m.Nominee,
                LocationId = m.LocationId,
                LocationName = m.Location!.Name
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateMember(int id, MemberRequestDto request)
    {
        var member = await context.Members.FindAsync(id);
        if (member == null)
            throw new InvalidOperationException("Member not found.");

        if (member.EmployeeNo != request.EmployeeNo)
        {
            var exists = context.Set<Member>()
                .Any(a => a.EmployeeNo == request.EmployeeNo);

            if (exists)
                throw new InvalidOperationException("Employee number must be unique.");
        }

        member.EmployeeNo = request.EmployeeNo;
        member.FirstName = request.FirstName;
        member.LastName = request.LastName;
        member.Email = request.Email;
        member.Phone = request.Phone;
        member.Share = request.Share;
        member.Nominee = request.Nominee;
        member.JoiningDate = request.JoiningDate;
        member.LocationId = request.LocationId;

        await context.SaveChangesAsync();
    }

    public async Task DeleteMember(int id)
    {
        var member = await context.Members.FindAsync(id);

        if (member != null)
        {
            var user = await userManager.FindByEmailAsync(member.Email);
            if (user != null)
            {
                await userManager.DeleteAsync(user);
            }
            context.Members.Remove(member);

            await context.SaveChangesAsync();
        }
    }
}
