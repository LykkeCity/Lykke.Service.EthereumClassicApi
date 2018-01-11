using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Lykke.Service.EthereumClassic.Api.Utils
{
    public class BigIntegerAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return BigInteger.TryParse(value.ToString(), out var _)
                ? ValidationResult.Success
                : new ValidationResult("Number is invalid.");
        }
    }
}