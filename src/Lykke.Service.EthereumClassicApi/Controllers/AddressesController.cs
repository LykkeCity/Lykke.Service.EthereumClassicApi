using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
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
