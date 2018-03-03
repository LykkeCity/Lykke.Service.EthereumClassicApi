using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("/api/transactions/history")]
    public class TransactionsHistoryController : Controller
    {
        [HttpPost("to/{address}")]
        public IActionResult AddAddressToIncomingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpPost("from/{address}")]
        public IActionResult AddAddressToOutgoingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpDelete("to/{address}")]
        public IActionResult DeleteAddressFromIncomingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpDelete("from/{address}")]
        public IActionResult DeleteAddressFromOutgoingObservationList(string address)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpGet("to/{address}")]
        public IActionResult GetIncomingHistory(string address, [FromQuery] int take, [FromQuery] string afterHash = "")
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpGet("from/{address}")]
        public IActionResult GetOutgoingHistory(string address, [FromQuery] int take, [FromQuery] string afterHash = "")
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }
    }
}
