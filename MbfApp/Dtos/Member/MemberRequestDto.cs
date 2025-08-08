using MbfApp.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MbfApp.Dtos.Member;

public class MemberRequestDto
{
    [Required, StringLength(4), EmployeeCode]
    public string EmployeeNo { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string FirstName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string LastName { get; set; } = string.Empty;

    [Required, StringLength(50)]
    public string Email { get; set; } = string.Empty;

    [Required, StringLength(10), Phone]
    public string Phone { get; set; } = string.Empty;
    [StringLength(100)]
    public string? Nominee { get; set; }
    public int Share { get; set; }

    [Required, DateNotInFuture, Column(TypeName = "date")]
    public DateTime JoiningDate { get; set; } = DateTime.Now;

    [DateNotInFuture, Column(TypeName = "date")]
    public DateTime? LeavingDate { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "Please select a location")]
    public int LocationId { get; set; }
}
