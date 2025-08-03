using MbfApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MbfApp.Data.Entities;

public class AccountCode
{
    public int Id { get; set; } 
      
    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
