using System.ComponentModel.DataAnnotations;
using Lykke.Service.EthereumClassicApi.Common.Utils;

namespace Lykke.Service.EthereumClassicApi.Utils
{
    public class AddressAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return AddressValidator.ValidateAsync(value.ToString()).Result
                ? ValidationResult.Success 
                : new ValidationResult("Address is invalid.");
        }
    }
}