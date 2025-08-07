using System.ComponentModel.DataAnnotations;

namespace MbfApp.Dtos.Locations;

public class LocationRequestDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
