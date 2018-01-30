using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionProcessorRole : ITransactionProcessorRole
    {
        private readonly IBroadcastedTransactionRepository _broadcastedTransactionRepository;
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly IBuiltTransactionRepository _builtTransactionRepository;
        private readonly IEthereum _ethereum;
        private readonly IObservableBalanceRepository _observableBalanceRepository;


        public TransactionProcessorRole(
            IBroadcastedTransactionRepository broadcastedTransactionRepository,
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            IBuiltTransactionRepository builtTransactionRepository,
            IEthereum ethereum,
            IObservableBalanceRepository observableBalanceRepository)
        {
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _broadcastedTransactionRepository = broadcastedTransactionRepository;
            _builtTransactionRepository = builtTransactionRepository;
            _ethereum = ethereum;
            _observableBalanceRepository = observableBalanceRepository;
        }
        
        /// <inheritdoc />
        public async Task<string> BroadcastTransaction(Guid operationId, string signedTxData)
        {
            if (!await _broadcastedTransactionRepository.ExistsAsync(operationId, signedTxData))
            {
                var operation = await _builtTransactionRepository.TryGetAsync(operationId);
                if (operation == null)
                {
                    throw new NotFoundException($"Specified operation [{operationId}] is not found.");
                }

                if (await _observableBalanceRepository.ExistsAsync(operation.FromAddress))
                {
                    await _observableBalanceRepository.UpdateAsync
                    (
                        address: operation.FromAddress,
                        amount: 0,
                        locked: true
                    );
                }

                var txHash = await SendRawTransactionOrGetTxHashAsync(signedTxData);
                var now = DateTime.UtcNow;

                await _broadcastedTransactionRepository.AddOrReplaceAsync(new BroadcastedTransactionDto
                {
                    Amount = operation.Amount,
                    FromAddress = operation.FromAddress,
                    OperationId = operationId,
                    SignedTxData = signedTxData,
                    Timestamp = now,
                    ToAddress = operation.ToAddress,
                    TxHash = txHash
                });
                
                await _broadcastedTransactionStateRepository.AddOrReplaceAsync(new BroadcastedTransactionStateDto
                {
                    Amount = operation.Amount,
                    Fee = new BigInteger(0),
                    FromAddress = operation.FromAddress,
                    OperationId = operationId,
                    State = TransactionState.InProgress,
                    Timestamp = now,
                    ToAddress = operation.ToAddress,
                    TxHash = txHash
                });
                
                return txHash;
            }

            throw new ConflictException(
                $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has laready been broadcasted.");
        }

        /// <summary>
        ///   Sends raw transaction, or, if it has already been sent, returns it's txHash
        /// </summary>
        /// <param name="signedTxData">
        ///    Signed transaction data.
        /// </param>
        /// <returns>
        ///    Transaction hash.
        /// </returns>
        private async Task<string> SendRawTransactionOrGetTxHashAsync(string signedTxData)
        {
            var txHash = _ethereum.GetTransactionHash(signedTxData);
            var receipt = await _ethereum.GetTransactionReceiptAsync(txHash);

            if (receipt == null)
            {
                await _ethereum.SendRawTransactionAsync(signedTxData);
            }

            return txHash;
        }
    }
}
