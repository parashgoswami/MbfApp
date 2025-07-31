using MbfApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MbfApp.Dtos.AccountCodes;

public class AccountCodeRequestDto
{
    [Required]
    public AccountType AccountType { get; set; }
    [Required,StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
