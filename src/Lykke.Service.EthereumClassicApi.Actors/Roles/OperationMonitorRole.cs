using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class OperationMonitorRole : IOperationMonitorRole
    {
        private readonly IOperationRepository            _operationRepository;
        private readonly IOperationStateRepository       _operationStateRepository;
        private readonly IOperationTransactionRepository _operationTransactionRepository;
        private readonly ITransactionStateService        _transactionStateService;


        public OperationMonitorRole(
            IOperationRepository operationRepository,
            IOperationStateRepository operationStateRepository,
            IOperationTransactionRepository operationTransactionRepository,
            ITransactionStateService transactionStateService)
        {
            _operationRepository            = operationRepository;
            _operationStateRepository       = operationStateRepository;
            _operationTransactionRepository = operationTransactionRepository;
            _transactionStateService        = transactionStateService;
        }


        public async Task<bool> CheckOperationAsync(Guid operationId)
        {
            var operationTransactions      = (await _operationTransactionRepository.GetAsync(operationId)).ToList();
            var operationTransactionStates = await Task.WhenAll(operationTransactions
                .Select(async x =>
                {
                    var state = await _transactionStateService.GetTransactionStateAsync(x.TxHash);

                    return new
                    {
                        OperationTransaction = x,
                        State                = state
                    };
                }));
            
            var completedTransactionStates = operationTransactionStates.Where(x => x.State.Completed).ToList();

            if (completedTransactionStates.Count > 1)
            {
                throw new UnsupportedEdgeCaseException($"More than one transaction completed for operation [{operationId:N}].");
            }

            if (completedTransactionStates.Count == 1)
            {
                var completedTransaction = completedTransactionStates.Single(x => x.State.Completed);
                var operationTransaction = completedTransaction.OperationTransaction;
                var state                = completedTransaction.State;

                await _operationStateRepository.AddOrReplaceAsync(new OperationStateDto
                {
                    Amount      = operationTransaction.Amount,
                    Completed   = true,
                    Error       = "Transaction failed during execution.",
                    Failed      = state.Failed,
                    Fee         = operationTransaction.Fee,
                    FromAddress = operationTransaction.FromAddress,
                    OperationId = operationId,
                    Timestamp   = operationTransaction.CreatedOn,
                    ToAddress   = operationTransaction.ToAddress,
                    TxHash      = operationTransaction.TxHash
                });

                await _operationStateRepository.DeleteInProgressAsync(operationId);

                await _operationRepository.DeleteAsync(operationId);

                await _operationTransactionRepository.DeleteAsync(operationId);

                return true;
            }
            else
            {
                foreach (var operationTransaction in operationTransactions)
                {
                    await _operationStateRepository.AddOrReplaceAsync(new OperationStateDto
                    {
                        Amount      = operationTransaction.Amount,
                        Fee         = operationTransaction.Fee,
                        FromAddress = operationTransaction.FromAddress,
                        OperationId = operationId,
                        Timestamp   = operationTransaction.CreatedOn,
                        ToAddress   = operationTransaction.ToAddress,
                        TxHash      = operationTransaction.TxHash
                    });
                }

                return false;
            }
        }
    }
}
