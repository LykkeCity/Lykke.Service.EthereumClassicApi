using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.EthereumClassic.Api.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassic.Api.Controllers
{
    [Route("/api/addresses")]
    public class AddressesController : Controller
    {
        [HttpGet("{address}/validity")]
        public async Task<IActionResult> GetAddressValidity(string address)
        {
            return Ok(new AddressValidationResponse
            {
                IsValid = await AddressValidator.ValidateAsync(address)
            });
        }
    }
}
