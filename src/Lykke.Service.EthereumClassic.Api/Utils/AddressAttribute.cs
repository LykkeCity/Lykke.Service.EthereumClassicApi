using System.ComponentModel.DataAnnotations;
using Lykke.Service.EthereumClassic.Api.Common.Utils;

namespace Lykke.Service.EthereumClassic.Api.Utils
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