using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Actors;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Filters;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace Lykke.Service.EthereumClassicApi.Controllers
{
    [Route("api/transactions")]
    public class TransactionsController : Controller
    {
        private readonly ITransactionService _transactionService;
        private readonly ITransactionRepository _transactionRepository;


        public TransactionsController(
            ITransactionService transactionService,
            ITransactionRepository transactionRepository)
        {
            _transactionService = transactionService;
            _transactionRepository = transactionRepository;
        }

        [HttpPost("broadcast")]
        public async Task<IActionResult> Broadcast([FromBody] BroadcastTransactionRequest request)
        {
            await _transactionService.BroadcastTransactionAsync
            (
                request.OperationId,
                request.SignedTransaction
            );
            
            return Ok();
        }


        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Build([FromBody] BuildTransactionRequest request)
        {
            var txParams = await _transactionService.CalculateTransactionParamsAsync
            (
                BigInteger.Parse(request.Amount),
                request.IncludeFee,
                request.ToAddress
            );

            if (txParams.Amount <= 0)
            {
                return BadRequest
                (
                    ErrorResponse.Create("Transaction amount should be greater then zero.")
                );
            }

            var txData = await _transactionService.BuildTransactionAsync
            (
                txParams.Amount,
                txParams.Fee,
                request.FromAddress.ToLowerInvariant(),
                txParams.GasPrice,
                request.IncludeFee,
                request.OperationId,
                request.ToAddress.ToLowerInvariant()
            );

            return Ok(new BuildTransactionResponse
            {
                TransactionContext = txData
            });
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
        [ValidateModel]
        public async Task<IActionResult> Rebuild([FromBody] RebuildTransactionRequest request)
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
    }
}

