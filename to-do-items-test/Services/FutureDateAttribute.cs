using System.ComponentModel.DataAnnotations;

public class FutureOrTodayDateAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Check if the value is a valid DateTime
        if (value is DateTime dateTime)
        {
            // Ensure the date is today or in the future (in UTC)
            if (dateTime.Date < DateTime.UtcNow.Date)
            {
                return new ValidationResult(ErrorMessage ?? "The date must be today or in the future.");
            }
        }

        // If the value is null or valid, return success
        return ValidationResult.Success;
    }
}
