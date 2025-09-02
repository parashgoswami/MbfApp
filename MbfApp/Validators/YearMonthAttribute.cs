using System.ComponentModel.DataAnnotations;

namespace MbfApp.Validators;

public class YearMonthAttribute : ValidationAttribute
{
    public YearMonthAttribute()
    {
        ErrorMessage = "The YearMonth format is invalid. It should be in YYYYMM format, e.g., 202501.";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success;
        }

        var yearMonthStr = value?.ToString();

        if (yearMonthStr?.Length == 6 && int.TryParse(yearMonthStr.AsSpan(0, 4), out int year) && int.TryParse(yearMonthStr.AsSpan(4, 2), out int month))
        {
            if (year > 1900 && year < 2100 && month >= 1 && month <= 12)
            {
                return ValidationResult.Success;
            }
        }

        return new ValidationResult(ErrorMessage);
    }
}

