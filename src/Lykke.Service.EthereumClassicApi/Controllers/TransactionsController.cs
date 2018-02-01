using System.Numerics;
using System.Threading.Tasks;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Extensions;
using Lykke.Service.EthereumClassicApi.Filters;
using Lykke.Service.EthereumClassicApi.Models;
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
        [ValidateModel]
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

        [HttpDelete("broadcast/{operationId:guid}")]
        [ValidateModel]
        public async Task<IActionResult> DeleteState(DeleteTransactionRequest request)
        {
            if (await _transactionRepository.DeleteIfExistsAsync(request.OperationId))
            {
                return Ok();
            }
            else
            {
                return NoContent();
            }
        }

        [HttpGet("broadcast/{operationId:guid}")]
        [ValidateModel]
        public async Task<IActionResult> GetState(GetTransactionStateRequest request)
        {
            var transactionState = await _transactionRepository.TryGetTransactionStateAsync(request.OperationId);

            if (transactionState != null)
            {
                return Ok(transactionState);
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

