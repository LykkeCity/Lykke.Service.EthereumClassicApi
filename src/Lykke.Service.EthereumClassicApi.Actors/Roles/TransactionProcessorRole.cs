using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionProcessorRole : ITransactionProcessorRole
    {
        private readonly IBroadcastedTransactionRepository _broadcastedTransactionRepository;
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly IBuiltTransactionRepository _builtTransactionRepository;
        private readonly IEthereum _ethereum;
        private readonly IGasPriceOracleService _gasPriceOracleService;
        private readonly IObservableBalanceLockRepository _observableBalanceLockRepository;


        public TransactionProcessorRole(
            IBroadcastedTransactionRepository broadcastedTransactionRepository,
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            IBuiltTransactionRepository builtTransactionRepository,
            IEthereum ethereum,
            IGasPriceOracleService gasPriceOracleService,
            IObservableBalanceLockRepository observableBalanceLockRepository)
        {
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _broadcastedTransactionRepository = broadcastedTransactionRepository;
            _builtTransactionRepository = builtTransactionRepository;
            _ethereum = ethereum;
            _gasPriceOracleService = gasPriceOracleService;
            _observableBalanceLockRepository = observableBalanceLockRepository;
        }
        
        /// <inheritdoc />
        public async Task<string> BroadcastTransaction(Guid operationId, string signedTxData)
        {
            if (!await _broadcastedTransactionRepository.ExistsAsync(operationId, signedTxData))
            {
                var operation = await _builtTransactionRepository.TryGetAsync(operationId);
                var txHash = await _ethereum.SendRawTransactionAsync(signedTxData);
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

                await _observableBalanceLockRepository.AddOrReplaceAsync(new ObservableBalanceLockDto
                {
                    Address = operation.FromAddress
                });

                return txHash;
            }

            throw new ConflictException(
                $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has laready been broadcasted.");
        }
    }
}
