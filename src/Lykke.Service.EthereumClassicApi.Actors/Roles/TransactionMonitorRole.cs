using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionMonitorRole : ITransactionMonitorRole
    {
        private readonly IBroadcastedTransactionRepository      _broadcastedTransactionRepository;
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly IBuiltTransactionRepository            _builtTransactionRepository;
        private readonly ITransactionStateService               _transactionStateService;


        public TransactionMonitorRole(
            IBroadcastedTransactionRepository broadcastedTransactionRepository,
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            IBuiltTransactionRepository builtTransactionRepository,
            ITransactionStateService transactionStateService)
        {
            _broadcastedTransactionRepository      = broadcastedTransactionRepository;
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _builtTransactionRepository            = builtTransactionRepository;
            _transactionStateService               = transactionStateService;
        }


        public async Task<bool> CheckTransactionStateAsync(Guid operationId)
        {
            var transactions      = (await _broadcastedTransactionRepository.GetAsync(operationId)).ToList();
            var transactionStates = await Task.WhenAll(transactions
                .Select(async x =>
                {
                    var state = await _transactionStateService.GetTransactionStateAsync(x.TxHash);

                    return new
                    {
                        Transaction = x,
                        State       = state
                    };
                }));
            
            var completedTransactionStates = transactionStates.Where(x => x.State.Completed).ToList();

            if (completedTransactionStates.Count > 1)
            {
                throw new UnsupportedEdgeCaseException($"More than one transaction completed for operation [{operationId:N}].");
            }

            if (completedTransactionStates.Count == 1)
            {
                var completedTransaction = completedTransactionStates.Single(x => x.State.Completed);
                var operationTransaction = completedTransaction.Transaction;
                var state                = completedTransaction.State.Failed ? TransactionState.Failed : TransactionState.Completed;


                await _broadcastedTransactionStateRepository.AddOrReplaceAsync(new BroadcastedTransactionStateDto
                {
                    Amount      = operationTransaction.Amount,
                    Error       = state == TransactionState.Completed ? "" : "Transaction failed during execution.", // TODO: Add better failure explanation
                    Fee         = operationTransaction.Fee,
                    FromAddress = operationTransaction.FromAddress,
                    OperationId = operationId,
                    State       = state,
                    Timestamp   = DateTimeOffset.UtcNow,
                    ToAddress   = operationTransaction.ToAddress,
                    TxHash      = operationTransaction.TxHash
                });
                
                await _builtTransactionRepository.DeleteAsync(operationId);

                await _broadcastedTransactionRepository.DeleteAsync(operationId);

                return true;
            }
            else
            {
                var latestTransaction = transactions.OrderByDescending(x => x.Timestamp).FirstOrDefault();

                if (latestTransaction != null)
                {
                    await _broadcastedTransactionStateRepository.AddOrReplaceAsync(new BroadcastedTransactionStateDto
                    {
                        Amount      = latestTransaction.Amount,
                        Fee         = latestTransaction.Fee,
                        FromAddress = latestTransaction.FromAddress,
                        OperationId = operationId,
                        State       = TransactionState.InProgress,
                        Timestamp   = latestTransaction.Timestamp,
                        ToAddress   = latestTransaction.ToAddress,
                        TxHash      = latestTransaction.TxHash
                    });
                }
                
                return false;
            }
        }

        public async Task DeleteTransactionStateAsync(Guid operationId)
        {
            await _broadcastedTransactionStateRepository.DeleteAsync(operationId);
        }
    }
}
