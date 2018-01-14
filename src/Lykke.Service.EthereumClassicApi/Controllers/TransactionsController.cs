using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly IActorSystemFacade          _actorSystemFacade;
        private readonly IOperationStateQueryService _operationStateQueryService;


        public TransactionsController(
            IActorSystemFacade actorSystemFacade,
            IOperationStateQueryService operationStateQueryService)
        {
            _actorSystemFacade          = actorSystemFacade;
            _operationStateQueryService = operationStateQueryService;
        }


        [HttpPost("broadcast")]
        public async Task<IActionResult> BroadcastSignedTransaction(BroadcastTransactionRequest request)
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
                return StatusCode((int) HttpStatusCode.Conflict, e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BuildUnsignedTransaction(BuildTransactionRequest request)
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

        [HttpDelete("observation")]
        public async Task<IActionResult> DeleteTransactionsFromObservationList([FromBody] IEnumerable<Guid> operationIds)
        {
            await _actorSystemFacade.DeleteOperationStates(operationIds);

            return Ok();
        }

        [HttpGet("completed")]
        public async Task<IActionResult> GetCompletedTransactionList([FromQuery] int take = 1000, [FromQuery] string continuation = "")
        {
            // TODO: Add actual pagination

            var transactions = (await _operationStateQueryService.GetCompletedOperationsAsync(take, continuation))
                .Select(x => new CompletedTransactionContract
                {
                    Amount      = x.Amount.ToString(),
                    AssetId     = Constants.EtcAsset.AssetId,
                    FromAddress = x.FromAddress,
                    Hash        = x.TxHash,
                    OperationId = x.OperationId,
                    Timestamp   = x.Timestamp.DateTime,
                    ToAddress   = x.ToAddress
                });

            return Ok(new PaginationResponse<CompletedTransactionContract>
            {
                Continuation = null,
                Items        = transactions.ToImmutableList()
            });
        }

        [HttpGet("failed")]
        public async Task<IActionResult> GetFailedTransactionList([FromQuery] int take = 1000, [FromQuery] string continuation = "")
        {
            // TODO: Add actual pagination

            var transactions = (await _operationStateQueryService.GetFailedOperationsAsync(take, continuation))
                .Select(x => new FailedTransactionContract
                {
                    Amount      = x.Amount.ToString(),
                    AssetId     = Constants.EtcAsset.AssetId,
                    Error       = x.Error,
                    FromAddress = x.FromAddress,
                    OperationId = x.OperationId,
                    Timestamp   = x.Timestamp.DateTime,
                    ToAddress   = x.ToAddress
                });

            return Ok(new PaginationResponse<FailedTransactionContract>
            {
                Continuation = null,
                Items        = transactions.ToImmutableList()
            });
        }

        [HttpGet("in-progress")]
        public async Task<IActionResult> GetInProgressTransactionList([FromQuery] int take = 1000, [FromQuery] string continuation = "")
        {
            // TODO: Add actual pagination

            var transactions = (await _operationStateQueryService.GetInProgressOperationsAsync(take, continuation))
                .Select(x => new InProgressTransactionContract
                {
                    Amount      = x.Amount.ToString(),
                    AssetId     = Constants.EtcAsset.AssetId,
                    FromAddress = x.FromAddress,
                    Hash        = x.TxHash,
                    OperationId = x.OperationId,
                    Timestamp   = x.Timestamp.DateTime,
                    ToAddress   = x.ToAddress
                });

            return Ok(new PaginationResponse<InProgressTransactionContract>
            {
                Continuation = null,
                Items        = transactions.ToImmutableList()
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateUnsignedTransaction(RebuildTransactionRequest request)
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
    }
}
