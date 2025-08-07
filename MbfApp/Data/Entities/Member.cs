using MbfApp.Validators;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MbfApp.Data.Entities;

public class Member
{
    public int Id { get; set; }

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

    [Required, DateNotInFuture, Column(TypeName = "date")]
    public DateTime JoiningDate { get; set; }

    [Column(TypeName = "date")]
    public DateTime? LeavingDate { get; set; }

    public bool IsActive { get; set; } = true;
    public int Share { get; set; }
    public string? Nominee { get; set; }

    public int LocationId { get; set; }
    public Location? Location { get; set; }
}
