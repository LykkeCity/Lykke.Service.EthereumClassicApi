using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.EthereumClassic.Api.Actors;
using Lykke.Service.EthereumClassic.Api.Common;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;
using Lykke.Service.EthereumClassic.Api.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassic.Api.Controllers
{
    [Route("api/balances")]
    public class BalancesController : Controller
    {
        private readonly IActorSystemFacade   _actorSystemFacade;
        private readonly IBalanceQueryService _balanceQueryService;


        public BalancesController(
            IActorSystemFacade actorSystemFacade,
            IBalanceQueryService balanceQueryService)
        {
            _actorSystemFacade   = actorSystemFacade;
            _balanceQueryService = balanceQueryService;
        }


        [HttpPost("{address}/observation")]
        public async Task<IActionResult> AddAddressToObservationList([Address] string address)
        {
            await _actorSystemFacade.BeginBalanceMonitoringAsync(address);

            return Ok();
        }

        [HttpDelete("{address}/observation")]
        public async Task<IActionResult> DeleteAddressFromObservationList([Address] string address)
        {
            await _actorSystemFacade.EndBalanceMonitoringAsync(address);

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetBalanceList([FromQuery] int take, [FromQuery] string continuation = "")
        {
            // TODO: Add actual pagination

            var balances = (await _balanceQueryService.GetBalancesAsync(take, continuation))
                .Select(x => new WalletBalanceContract
                {
                    Address = x.Address,
                    AssetId = Constants.EtcAsset.AssetId,
                    Balance = x.Balance.ToString()
                });

            return Ok(new PaginationResponse<WalletBalanceContract>
            {
                Continuation = null,
                Items        = balances.ToImmutableList()
            });
        }
    }
}
