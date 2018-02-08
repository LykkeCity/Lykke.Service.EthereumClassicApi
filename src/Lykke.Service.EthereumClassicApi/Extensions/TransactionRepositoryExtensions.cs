using System;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.BlockchainApi.Contract.Transactions;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Extensions
{
    internal static class TransactionRepositoryExtensions
    {
        public static async Task<BroadcastedTransactionResponse> TryGetTransactionStateAsync(this ITransactionRepository transactionRepository, Guid operationId)
        {
            var transactions = (await transactionRepository.GetAllAsync(operationId))
                .ToList();

            TransactionEntity transaction = null;

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

                return new BroadcastedTransactionResponse
                {
                    Amount = transaction.Amount.ToString(),
                    Error = transaction.Error,
                    Fee = transaction.Fee.ToString(),
                    Hash = transaction.SignedTxHash,
                    OperationId = transaction.OperationId,
                    State = state,
                    Timestamp = transaction.CompletedOn ?? transaction.BroadcastedOn ?? transaction.BuiltOn
                };
            }
            else
            {
                return null;
            }
        }
    }
}
