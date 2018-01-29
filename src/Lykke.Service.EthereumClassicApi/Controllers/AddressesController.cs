using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Addresses;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("/api/addresses")]
    public class AddressesController : Controller
    {
        private readonly IAddressValidationService _addressValidationService;


        public AddressesController(
            IAddressValidationService addressValidationService)
        {
            _addressValidationService = addressValidationService;
        }


        [HttpGet("{address}/validity")]
        public async Task<IActionResult> GetAddressValidity(string address)
        {
            return Ok(new AddressValidationResponse
            {
                IsValid = await _addressValidationService.ValidateAddressAsync(address)
            });
        }
    }
}
