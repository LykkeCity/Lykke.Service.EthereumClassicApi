using System.Net;
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


        [HttpPost("single")]
        [ValidateModel]
        public async Task<IActionResult> Build([FromBody] BuildSingleTransactionRequest request)
        {
            var txParams = await _transactionService.CalculateTransactionParamsAsync
            (
                BigInteger.Parse(request.Amount),
                request.IncludeFee,
                request.ToAddress.ToLowerInvariant()
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

        [HttpPost("many-inputs")]
        public IActionResult Build([FromBody] BuildTransactionWithManyInputsRequest request)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
        }

        [HttpPost("many-outputs")]
        public IActionResult Build([FromBody] BuildTransactionWithManyOutputsRequest request)
        {
            return StatusCode((int) HttpStatusCode.NotImplemented);
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

        [HttpGet("broadcast/single/{operationId:guid}")]
        [ValidateModel]
        public async Task<IActionResult> GetSingleTransactionState(GetTransactionStateRequest request)
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

        [HttpGet("broadcast/many-inputs/{operationId:guid}")]
        public IActionResult GetManyInputsTransactionState(GetTransactionStateRequest request)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
        }

        [HttpGet("broadcast/many-outputs/{operationId:guid}")]
        public IActionResult GetManyOutputsTransactionState(GetTransactionStateRequest request)
        {
            return StatusCode((int)HttpStatusCode.NotImplemented);
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

