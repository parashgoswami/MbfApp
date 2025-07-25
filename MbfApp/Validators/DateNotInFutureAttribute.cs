using System.ComponentModel.DataAnnotations;

namespace MbfApp.Validators;

public class DateNotInFutureAttribute : ValidationAttribute
{
    public DateNotInFutureAttribute()
    {
        ErrorMessage = "Joining date cannot be in the future.";
    }
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime date)
        {
            if (date > DateTime.Today)
            {
                return new ValidationResult(ErrorMessage);
            }
        }

        return ValidationResult.Success;
    }
}
