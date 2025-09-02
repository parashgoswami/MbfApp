using System.ComponentModel.DataAnnotations;

namespace MbfApp.Validators;

public class EmployeeCodeAttribute : ValidationAttribute
{
    public EmployeeCodeAttribute()
    {
        ErrorMessage = "The employee code must be exactly 4 digits.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
            return ValidationResult.Success; // Let [Required] handle nulls

        string? strValue = value.ToString();

        return strValue?.Length == 4 && int.TryParse(strValue, out _)
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }
}
