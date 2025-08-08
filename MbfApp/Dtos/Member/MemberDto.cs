namespace MbfApp.Dtos.Member;

public class MemberDto
{
    public int Id { get; set; }
    public string EmployeeNo { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime JoiningDate { get; set; }
    public DateTime LeavingDate { get; set; }
    public bool IsActive { get; set; }
    public int Share { get; set; }
    public string? Nominee { get; set; }
    public int LocationId { get; set; }
    public string? LocationName { get; set; }
}