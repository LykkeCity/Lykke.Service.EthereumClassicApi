using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionMonitorRole : ITransactionMonitorRole
    {
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionStateService _transactionStateService;


        public TransactionMonitorRole(
            IObservableBalanceRepository observableBalanceRepository,
            ITransactionRepository transactionRepository,
            ITransactionStateService transactionStateService)
        {
            _observableBalanceRepository = observableBalanceRepository;
            _transactionRepository = transactionRepository;
            _transactionStateService = transactionStateService;
        }


        public async Task<bool> CheckTransactionStatesAsync(Guid operationId)
        {
            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId)).ToList();

            bool TransactionCompleted(TransactionDto dto)
            {
                return dto.State == TransactionState.Completed ||
                       dto.State == TransactionState.Failed;
            }

            foreach (var operationTransaction in operationTransactions.Where(x => x.State == TransactionState.InProgress))
            {
                var currentState = await _transactionStateService.GetTransactionStateAsync(operationTransaction.SignedTxHash);

                operationTransaction.Error = currentState.Error;
                operationTransaction.State = currentState.State;

                if (TransactionCompleted(operationTransaction))
                {
                    //TODO: Use timestamp of a block

                    operationTransaction.CompletedOn = DateTime.UtcNow;
                }
            }

            var completedTransactions = operationTransactions.Where(TransactionCompleted).ToList();
            if (completedTransactions.Count > 1)
            {
                throw new UnsupportedEdgeCaseException($"More than one transaction completed for operation [{operationId}].");
            }

            var completedTransaction = completedTransactions.FirstOrDefault();
            if (completedTransaction != null)
            {
                await UnlockBalanceIfNecessaryAsync(completedTransaction.FromAddress);

                await _transactionRepository.UpdateAsync(new CompletedTransactionDto
                { 
                    // ReSharper disable once PossibleInvalidOperationException
                    // CompletedOn should always be set for completed or failed transactions
                    CompletedOn = completedTransaction.CompletedOn.Value,
                    Error = completedTransaction.Error,
                    OperationId = operationId,
                    State = completedTransaction.State,
                    TxData = completedTransaction.TxData
                });

                return true;
            }
            else
            {
                return false;
            }
        }

        private async Task UnlockBalanceIfNecessaryAsync(string fromAddress)
        {
            if (await _observableBalanceRepository.ExistsAsync(fromAddress))
            {
                await _observableBalanceRepository.UpdateLockAsync(fromAddress, true);
            }
        }
    }
}
