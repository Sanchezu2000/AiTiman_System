using System.ComponentModel.DataAnnotations;

namespace AiTiman_System.Attributes
{
    public class LapayArgaoAddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is string address && address.Equals("Lapay,Argao", StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }
            return new ValidationResult("Address not Accepted");
        }
    }
}
