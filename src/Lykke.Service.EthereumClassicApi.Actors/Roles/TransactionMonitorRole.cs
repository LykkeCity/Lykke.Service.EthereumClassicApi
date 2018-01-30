﻿using System;
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
        private readonly IBroadcastedTransactionRepository _broadcastedTransactionRepository;
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly IBuiltTransactionRepository _builtTransactionRepository;
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly ITransactionStateService _transactionStateService;


        public TransactionMonitorRole(
            IBroadcastedTransactionRepository broadcastedTransactionRepository,
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            IBuiltTransactionRepository builtTransactionRepository,
            IObservableBalanceRepository observableBalanceRepository,
            ITransactionStateService transactionStateService)
        {
            _broadcastedTransactionRepository = broadcastedTransactionRepository;
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _builtTransactionRepository = builtTransactionRepository;
            _observableBalanceRepository = observableBalanceRepository;
            _transactionStateService = transactionStateService;
        }


        public async Task<bool> CheckTransactionStateAsync(Guid operationId)
        {
            var transactions = (await _broadcastedTransactionRepository.GetAsync(operationId)).ToList();
            var transactionStates = await Task.WhenAll(transactions
                .Select(async x =>
                {
                    var state = await _transactionStateService.GetTransactionStateAsync(x.TxHash);

                    return new
                    {
                        Transaction = x,
                        State = state
                    };
                }));

            var finishedTransactionStates =
                transactionStates.Where(x => x.State.State != TransactionState.InProgress).ToList();

            if (finishedTransactionStates.Count > 1)
            {
                throw new UnsupportedEdgeCaseException(
                    $"More than one transaction finished for operation [{operationId}].");
            }
            else if (finishedTransactionStates.Count == 1)
            {
                var completedTransactionState = finishedTransactionStates.Single(x => x.State.State != TransactionState.InProgress);
                var completedTransaction = completedTransactionState.Transaction;
                var state = completedTransactionState.State;

                if (await _observableBalanceRepository.ExistsAsync(completedTransaction.FromAddress))
                {
                    await _observableBalanceRepository.UpdateAsync
                    (
                        address: completedTransaction.FromAddress,
                        amount: 0,
                        locked: false
                    );
                }

                await _broadcastedTransactionStateRepository.AddOrReplaceAsync(new BroadcastedTransactionStateDto
                {
                    Amount = completedTransaction.Amount,
                    Error = state.Error,
                    Fee = state.Fee,
                    FromAddress = completedTransaction.FromAddress,
                    OperationId = operationId,
                    State = state.State,
                    Timestamp = DateTime.UtcNow,
                    ToAddress = completedTransaction.ToAddress,
                    TxHash = completedTransaction.TxHash
                });
                
                await _builtTransactionRepository.DeleteIfExistsAsync(operationId);

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
                        Amount = latestTransaction.Amount,
                        Fee = null,
                        FromAddress = latestTransaction.FromAddress,
                        OperationId = operationId,
                        State = TransactionState.InProgress,
                        Timestamp = latestTransaction.Timestamp,
                        ToAddress = latestTransaction.ToAddress,
                        TxHash = latestTransaction.TxHash
                    });
                }

                return false;
            }
        }

        public async Task DeleteTransactionStateAsync(Guid operationId)
        {
            await _broadcastedTransactionStateRepository.DeleteIfExistAsync(operationId);
        }
    }
}
