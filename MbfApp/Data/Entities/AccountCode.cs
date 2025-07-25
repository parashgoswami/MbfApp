using System.ComponentModel.DataAnnotations;

namespace MbfApp.Data.Entities;

public class AccountCode
{
    public int Id { get; set; }
    [Required]
    [RegularExpression(@"^\d{4}$", ErrorMessage = "Employee number must be exactly 4 digits.")]
    public string Code { get; set; } = string.Empty;
    [Required]
    [StringLength(50)]
    public string Description { get; set; } = string.Empty;
}
