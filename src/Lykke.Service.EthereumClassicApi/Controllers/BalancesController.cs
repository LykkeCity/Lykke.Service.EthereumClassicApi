using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Utils;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/balances")]
    public class BalancesController : Controller
    {
        private readonly IObservableBalanceRepository _observableBalanceRepository;


        public BalancesController(
            IObservableBalanceRepository observableBalanceRepository)
        {
            _observableBalanceRepository = observableBalanceRepository;
        }


        [HttpPost("{address}/observation")]
        public async Task<IActionResult> AddAddressToObservationList([Address] string address)
        {
            if (await _observableBalanceRepository.TryAddAsync(address))
            {
                return Ok();
            }
            else
            {
                return StatusCode
                (
                    (int)HttpStatusCode.Conflict,
                    $"Specified address [{address}] is already observed."
                );
            }
        }

        [HttpDelete("{address}/observation")]
        public async Task<IActionResult> DeleteAddressFromObservationList([Address] string address)
        {
            if (await _observableBalanceRepository.DeleteIfExistsAsync(address))
            {
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBalanceList([FromQuery] int take, [FromQuery] string continuation = "")
        {
            (var balances, var continuationToken) = await _observableBalanceRepository.GetAllWithNonZeroAmountAsync(take, continuation);

            var responseItems = balances
                .Select(x => new WalletBalanceContract
                {
                    Address = x.Address.ToLowerInvariant(),
                    AssetId = Constants.EtcAsset.AssetId,
                    Balance = x.Amount.ToString()
                })
                .ToImmutableList();

            return Ok(new PaginationResponse<WalletBalanceContract>
            {
                Continuation = continuationToken,
                Items = responseItems
            });
        }
    }
}
