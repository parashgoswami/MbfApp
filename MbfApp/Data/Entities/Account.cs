using MbfApp.Data.Enums;
using System.ComponentModel.DataAnnotations;

namespace MbfApp.Data.Entities;

public class Account
{
    public int Id { get; set; } 
      
    [Required]
    public AccountType AccountType { get; set; }

    [Required]
    [StringLength(50)]
    public string AccountLabel { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}
