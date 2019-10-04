using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Extensions;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionMonitorRole : ITransactionMonitorRole
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITransactionStateService _transactionStateService;
        
        public TransactionMonitorRole(
            ITransactionRepository transactionRepository,
            ITransactionStateService transactionStateService)
        {
            _transactionRepository = transactionRepository;
            _transactionStateService = transactionStateService;
        }


        public async Task<bool> CheckTransactionStatesAsync(Guid operationId)
        {
            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId)).ToList();
            
            foreach (var operationTransaction in operationTransactions.Where(x => x.State == TransactionState.InProgress))
            {
                try
                {
                    var currentState = await _transactionStateService.GetTransactionStateAsync(operationTransaction.SignedTxHash);
                    
                    operationTransaction.BlockNumber = currentState.BlockNumber;
                    operationTransaction.CompletedOn = currentState.CompletedOn;
                    operationTransaction.Error = currentState.Error;
                    operationTransaction.State = currentState.State;
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to get state of the transaction {operationTransaction.SignedTxHash}", ex);
                }
            }

            var completedTransactions = operationTransactions.Where(x => x.IsFinished()).ToList();
            if (completedTransactions.Count > 1)
            {
                throw new UnsupportedEdgeCaseException($"More than one transaction completed for operation [{operationId}].");
            }

            var completedTransaction = completedTransactions.FirstOrDefault();
            if (completedTransaction != null)
            {
                await _transactionRepository.UpdateAsync(new CompletedTransactionDto
                {
                    // CompletedOn and BlockNumber should always be set for finished transactions
                    // ReSharper disable PossibleInvalidOperationException
                    BlockNumber = completedTransaction.BlockNumber.Value,
                    CompletedOn = completedTransaction.CompletedOn.Value,
                    // ReSharper restore PossibleInvalidOperationException
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
    }
}
