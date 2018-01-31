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
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly IActorSystemFacade _actorSystemFacade;
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;


        public TransactionsController(
            IActorSystemFacade actorSystemFacade,
            ITransactionService transactionService,
            ITransactionRepository transactionRepository)
        {
            _actorSystemFacade = actorSystemFacade;
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastTransactionRequest request)
        {
            try
            {
                await _transactionService.BroadcastTransactionAsync
                (
                    request.OperationId,
                    request.SignedTransaction
                );

                _actorSystemFacade.OnTransactionBroadcasted(request.OperationId);

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


            if (errorResponse.ModelErrors == null || !errorResponse.ModelErrors.Any())
            {
                try
                {
                    //var txData = await _transactionService.BuildTransactionAsync
                    //(
                    //    BigInteger.Parse(request.Amount),
                    //    request.FromAddress.ToLowerInvariant(),
                    //    request.IncludeFee,
                    //    request.OperationId,
                    //    request.ToAddress.ToLowerInvariant()
                    //);

                    return Ok(new BuildTransactionResponse
                    {
                        //TransactionContext = txData
                    });
                }
                catch (BadRequestException e)
                {
                    errorResponse.ErrorMessage = e.Message;
                }
            }

            return BadRequest(errorResponse);
        }

        [HttpDelete("broadcast/{operationId}")]
        public async Task<IActionResult> DeleteState(Guid operationId)
        {
            if (await _transactionRepository.DeleteIfExistsAsync(operationId))
            {
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
            var transactions = (await _transactionRepository.GetAllAsync(operationId))
                .ToList();

            TransactionDto transaction = null;

            var completedTransaction = transactions
                .SingleOrDefault(x => x.State == TransactionState.Completed || x.State == TransactionState.Failed);

            if (completedTransaction != null)
            {
                transaction = completedTransaction;
            }
            else
            {
                var latestInProgressTransaction = transactions
                    .Where(x => x.State == TransactionState.InProgress)
                    .OrderByDescending(x => x.BroadcastedOn)
                    .FirstOrDefault();

                if (latestInProgressTransaction != null)
                {
                    transaction = latestInProgressTransaction;
                }
            }

            if (transaction != null)
            {
                BroadcastedTransactionState state;

                switch (transaction.State)
                {
                    case TransactionState.InProgress:
                        state = BroadcastedTransactionState.InProgress;
                        break;
                    case TransactionState.Completed:
                        state = BroadcastedTransactionState.Completed;
                        break;
                    case TransactionState.Failed:
                        state = BroadcastedTransactionState.Failed;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return Ok(new BroadcastedTransactionResponse
                {
                    Amount = transaction.Amount.ToString(),
                    Error = transaction.Error,
                    Fee = transaction.Fee.ToString(),
                    Hash = transaction.SignedTxHash,
                    OperationId = transaction.OperationId,
                    State = state,
                    Timestamp = transaction.CompletedOn ?? transaction.BroadcastedOn ?? transaction.BuiltOn
                });
            }
            else
            {
                return NoContent();
            }
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
                var txData = await _transactionService.RebuildTransactionAsync
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
