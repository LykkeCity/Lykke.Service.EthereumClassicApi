using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Balances;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Filters;
using Lykke.Service.EthereumClassicApi.Models;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
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
        [ValidateModel]
        public async Task<IActionResult> AddAddressToObservationList(AddAddressToObservationListRequest request)
        {
            if (await _observableBalanceRepository.TryAddAsync(request.Address))
            {
                return Ok();
            }
            else
            {
                return StatusCode
                (
                    (int)HttpStatusCode.Conflict,
                    $"Specified address [{request.Address}] is already observed."
                );
            }
        }

        [HttpDelete("{address}/observation")]
        [ValidateModel]
        public async Task<IActionResult> DeleteAddressFromObservationList(DeleteAddressFromObservationListRequest request)
        {
            if (await _observableBalanceRepository.DeleteIfExistsAsync(request.Address))
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
                    Balance = x.Amount.ToString(),
                    Block = (long) x.BlockNumber
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
