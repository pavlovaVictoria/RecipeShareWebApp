using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Common.Custom_Validations
{
    public class MinutesForPrepCustomValidation : ValidationAttribute
    {
        //Here I check if the given int is multiple of 5
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is int intValue)
            {
                if ((intValue % 5 == 0))
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("The value must multiple of 5.");
        }
    }
}
