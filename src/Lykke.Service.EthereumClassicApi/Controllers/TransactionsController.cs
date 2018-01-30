using System;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Actors;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Mappers;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly IActorSystemFacade _actorSystemFacade;
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly ITransactionBuilderService _transactionBuilderService;


        public TransactionsController(
            IActorSystemFacade actorSystemFacade,
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            ITransactionBuilderService transactionBuilderService)
        {
            _actorSystemFacade = actorSystemFacade;
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _transactionBuilderService = transactionBuilderService;
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastTransactionRequest request)
        {
            try
            {
                await _actorSystemFacade.BroadcastTransactionAsync
                (
                    request.OperationId,
                    request.SignedTransaction
                );

                return Ok();
            }
            catch (ConflictException e)
            {
                return StatusCode((int) HttpStatusCode.Conflict, e.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Build([FromBody] BuildTransactionRequest request)
        {
            var errorResponse = new ErrorResponse();

            if (!BigInteger.TryParse(request.Amount, out var amount) || amount <= 0)
            {
                errorResponse.AddModelError("Amount", $"Amount [{request.Amount}] should be a positive integer.");
            }

            if (!await AddressValidator.ValidateAsync(request.FromAddress))
            {
                errorResponse.AddModelError("FromAddress",
                    $"FromAddress [{request.FromAddress}] should be a valid address.");
            }

            if (request.AssetId != Constants.EtcAsset.AssetId)
            {
                errorResponse.AddModelError("AssetId", $"AssetId [{request.AssetId}] is not supported.");
            }

            if (!await AddressValidator.ValidateAsync(request.ToAddress))
            {
                errorResponse.AddModelError("ToAddress", $"ToAddress [{request.ToAddress}] should be a valid address.");
            }


            if (errorResponse.ModelErrors == null || !errorResponse.ModelErrors.Any())
            {
                var txData = await _transactionBuilderService.BuildTransactionAsync
                (
                    BigInteger.Parse(request.Amount),
                    request.FromAddress,
                    request.IncludeFee,
                    request.OperationId,
                    request.ToAddress
                );

                return Ok(new BuildTransactionResponse
                {
                    TransactionContext = txData
                });
            }

            return BadRequest(errorResponse);
        }

        [HttpDelete("broadcast/{operationId}")]
        public async Task<IActionResult> DeleteState(Guid operationId)
        {
            if (await _broadcastedTransactionStateRepository.ExistsAsync(operationId))
            {
                await _broadcastedTransactionStateRepository.DeleteIfExistAsync(operationId);

                return Ok();
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("broadcast/{operationId}")]
        public async Task<IActionResult> GetState(Guid operationId)
        {
            var state = await _broadcastedTransactionStateRepository.TryGetAsync(operationId);

            if (state != null)
            {
                return Ok(state.ToBuildTransactionResponse());
            }

            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> Rebuild([FromBody] RebuildTransactionRequest request)
        {
            var errorResponse = new ErrorResponse();

            if (request.FeeFactor <= 1m)
            {
                errorResponse.AddModelError("FeeFactor", $"FeeFactor [{request.FeeFactor}] should be greater then 1.");
            }


            if (errorResponse.ModelErrors == null || !errorResponse.ModelErrors.Any())
            {
                var txData = await _transactionBuilderService.RebuildTransactionAsync
                (
                    request.FeeFactor,
                    request.OperationId
                );

                return Ok(new RebuildTransactionResponse
                {
                    TransactionContext = txData
                });
            }

            return BadRequest(errorResponse);
        }
    }
}
