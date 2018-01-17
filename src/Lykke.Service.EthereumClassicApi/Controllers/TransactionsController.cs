using System;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Actors;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Mappers;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly IActorSystemFacade                          _actorSystemFacade;
        private readonly IBroadcastedTransactionStateQueryRepository _broadcastedTransactionStateQueryRepository;


        public TransactionsController(
            IActorSystemFacade actorSystemFacade,
            IBroadcastedTransactionStateQueryRepository broadcastedTransactionStateQueryRepository)
        {
            _actorSystemFacade             = actorSystemFacade;
            _broadcastedTransactionStateQueryRepository = broadcastedTransactionStateQueryRepository;
        }


        [HttpPost]
        public async Task<IActionResult> Build(BuildTransactionRequest request)
        {
            var errorResponse = new ErrorResponse();

            if (!BigInteger.TryParse(request.Amount, out var amount) || amount <= 0)
            {
                errorResponse.AddModelError("Amount", $"Amount [{request.Amount}] should be a positive integer.");
            }

            if (!await AddressValidator.ValidateAsync(request.FromAddress))
            {
                errorResponse.AddModelError("FromAddress", $"FromAddress [{request.FromAddress}] should be a valid address.");
            }

            if (request.AssetId != Constants.EtcAsset.AssetId)
            {
                errorResponse.AddModelError("AssetId", $"AssetId [{request.AssetId}] is not supported.");
            }

            if (!await AddressValidator.ValidateAsync(request.ToAddress))
            {
                errorResponse.AddModelError("ToAddress", $"ToAddress [{request.ToAddress}] should be a valid address.");
            }


            if (!errorResponse.ModelErrors.Any())
            {
                var txData = await _actorSystemFacade.BuildTransactionAsync
                (
                    amount:      BigInteger.Parse(request.Amount),
                    fromAddress: request.FromAddress,
                    includeFee:  request.IncludeFee,
                    operationId: request.OperationId,
                    toAddress:   request.ToAddress
                );

                return Ok(new BuildTransactionResponse
                {
                    TransactionContext = txData
                });
            }
            else
            {
                return BadRequest(errorResponse);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Rebuild(RebuildTransactionRequest request)
        {
            var errorResponse = new ErrorResponse();

            if (request.FeeFactor <= 1m)
            {
                errorResponse.AddModelError("FeeFactor", $"FeeFactor [{request.FeeFactor}] should be greater then 1.");
            }


            if (!errorResponse.ModelErrors.Any())
            {
                var txData = await _actorSystemFacade.RebuildTransactionAsync
                (
                    feeFactor:   request.FeeFactor,
                    operationId: request.OperationId
                );

                return Ok(new RebuildTransactionResponse
                {
                    TransactionContext = txData
                });
            }
            else
            {
                return BadRequest(errorResponse);
            }
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast(BroadcastTransactionRequest request)
        {
            try
            {
                await _actorSystemFacade.BroadcastTransactionAsync
                (
                    operationId:  request.OperationId,
                    signedTxData: request.SignedTransaction
                );

                return Ok();
            }
            catch (ConflictException e)
            {
                return StatusCode((int)HttpStatusCode.Conflict, e.Message);
            }
        }

        [HttpGet("broadcast/{operationId}")]
        public async Task<IActionResult> GetState(Guid operationId)
        {
            var state = await _broadcastedTransactionStateQueryRepository.GetAsync(operationId);

            if (state != null)
            {
                return Ok(state.ToBuildTransactionResponse());
            }
            else
            {
                return NoContent();
            }
        }

        [HttpDelete("broadcast/{operationId}")]
        public async Task<IActionResult> DeleteState(Guid operationId)
        {
            try
            {
                await _actorSystemFacade.DeleteOperationStateAsync(operationId);

                return Ok();
            }
            catch (NotFoundException)
            {
                return NoContent();
            }
        }
    }
}
