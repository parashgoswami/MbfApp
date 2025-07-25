using System.ComponentModel.DataAnnotations;

namespace MbfApp.Data.Entities;

public class Location
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
}
