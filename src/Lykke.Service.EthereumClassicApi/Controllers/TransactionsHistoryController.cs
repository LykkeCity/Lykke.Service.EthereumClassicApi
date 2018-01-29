using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("/api/transactions/history")]
    public class TransactionsHistoryController : Controller
    {
        [HttpPost("to/{address}")]
        public async Task<IActionResult> AddAddressToIncomingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpPost("from/{address}")]
        public async Task<IActionResult> AddAddressToOutgoingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpDelete("to/{address}")]
        public async Task<IActionResult> DeleteAddressFromIncomingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpDelete("from/{address}")]
        public async Task<IActionResult> DeleteAddressFromOutgoingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpGet("to/{address}")]
        public async Task<IActionResult> GetIncomingHistory(string address, [FromQuery] int take,
            [FromQuery] string afterHash = "")
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpGet("from/{address}")]
        public async Task<IActionResult> GetOutgoingHistory(string address, [FromQuery] int take,
            [FromQuery] string afterHash = "")
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }
    }
}
