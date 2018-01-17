using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.EthereumClassicApi.Actors;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Utils;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/balances")]
    public class BalancesController : Controller
    {
        private readonly IActorSystemFacade                _actorSystemFacade;
        private readonly IObservableBalanceQueryRepository _observableBalanceQueryRepository;


        public BalancesController(
            IActorSystemFacade actorSystemFacade,
            IObservableBalanceQueryRepository observableBalanceQueryRepository)
        {
            _actorSystemFacade                = actorSystemFacade;
            _observableBalanceQueryRepository = observableBalanceQueryRepository;
        }


        [HttpPost("{address}/observation")]
        public async Task<IActionResult> AddAddressToObservationList([Address] string address)
        {
            try
            {
                await _actorSystemFacade.BeginBalanceMonitoringAsync(address);

                return Ok();
            }
            catch (ConflictException e)
            {
                return StatusCode((int) HttpStatusCode.Conflict, e.Message);
            }
        }

        [HttpDelete("{address}/observation")]
        public async Task<IActionResult> DeleteAddressFromObservationList([Address] string address)
        {
            try
            {
                await _actorSystemFacade.EndBalanceMonitoringAsync(address);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NoContent();
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBalanceList([FromQuery] int take, [FromQuery] string continuation = "")
        {
            (var balances, var continuationToken) = await _observableBalanceQueryRepository.GetAllWithNonZeroAmountAsync(take, continuation);
            
            var responseItems = balances
                .Select(x => new WalletBalanceContract
                {
                    Address = x.Address,
                    AssetId = Constants.EtcAsset.AssetId,
                    Balance = x.Amount.ToString()
                })
                .ToImmutableList();

            return Ok(new PaginationResponse<WalletBalanceContract>
            {
                Continuation = continuationToken,
                Items        = responseItems
            });
        }
    }
}
