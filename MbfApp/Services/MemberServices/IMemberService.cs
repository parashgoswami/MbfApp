using MbfApp.Dtos;
using MbfApp.Dtos.Member;

namespace MbfApp.Services;

public interface IMemberService
{
    public Task<PaginatedList<MemberDto>> GetPaginatedListAync(MemberSearchParameters searchParams);
    public Task<MemberDto?> GetMemberByIdAsync(int id);  
    public Task CreateMember(MemberRequestDto request);
    public Task UpdateMember(int id, MemberRequestDto request);
    public Task DeleteMember(int id);
}

