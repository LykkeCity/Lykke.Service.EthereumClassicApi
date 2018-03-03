using Lykke.Service.BlockchainApi.Contract.Common;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/capabilities")]
    public class CapabilitiesController : Controller
    {
        private static readonly CapabilitiesResponse CapabilitiesResponse;

        static CapabilitiesController()
        {
            CapabilitiesResponse = new CapabilitiesResponse
            {
                AreManyInputsSupported = false,
                AreManyOutputsSupported = false,
                IsTransactionsRebuildingSupported = true
            };
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(CapabilitiesResponse);
        }
    }
}
