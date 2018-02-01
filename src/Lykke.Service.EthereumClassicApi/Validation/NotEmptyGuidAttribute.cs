using System;
using System.ComponentModel.DataAnnotations;

namespace Lykke.Service.EthereumClassicApi.Validation
{
    public class NotEmptyGuidAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Guid guidValue && guidValue == Guid.Empty)
            {
                return new ValidationResult($"Should not be empty");
            }

            return ValidationResult.Success;
        }
    }
}
