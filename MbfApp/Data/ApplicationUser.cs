using Microsoft.AspNetCore.Identity;

namespace MbfApp.Data;

public class ApplicationUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public int MemberId { get; set; }
}
