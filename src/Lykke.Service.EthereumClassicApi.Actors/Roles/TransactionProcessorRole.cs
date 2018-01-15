using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class TransactionProcessorRole : ITransactionProcessorRole
    {
        private readonly IBroadcastedTransactionRepository      _broadcastedTransactionRepository;
        private readonly IBroadcastedTransactionStateRepository _broadcastedTransactionStateRepository;
        private readonly IBuiltTransactionRepository            _builtTransactionRepository;
        private readonly IEthereum                              _ethereum;
        private readonly IGasPriceOracleService                 _gasPriceOracleService;


        public TransactionProcessorRole(
            IBroadcastedTransactionRepository broadcastedTransactionRepository,
            IBroadcastedTransactionStateRepository broadcastedTransactionStateRepository,
            IBuiltTransactionRepository builtTransactionRepository,
            IEthereum ethereum,
            IGasPriceOracleService gasPriceOracleService)
        {
            _broadcastedTransactionStateRepository = broadcastedTransactionStateRepository;
            _broadcastedTransactionRepository      = broadcastedTransactionRepository;
            _builtTransactionRepository            = builtTransactionRepository;
            _ethereum                              = ethereum;
            _gasPriceOracleService                 = gasPriceOracleService;
        }

        /// <inheritdoc />
        public async Task<string> BroadcastTransaction(Guid operationId, string signedTxData)
        {
            if (!await _broadcastedTransactionRepository.ExistsAsync(operationId, signedTxData))
            {
                var operation = await _builtTransactionRepository.GetAsync(operationId);
                var txHash    = await _ethereum.SendRawTransactionAsync(signedTxData);
                var now       = DateTimeOffset.UtcNow;

                await _broadcastedTransactionRepository.AddAsync(new BroadcastedTransactionDto
                {
                    Amount       = operation.Amount,
                    Fee          = new BigInteger(), // TODO: Set fee
                    FromAddress  = operation.FromAddress,
                    OperationId  = operationId,
                    SignedTxData = signedTxData,
                    Timestamp    = now,
                    ToAddress    = operation.ToAddress,
                    TxHash       = txHash
                });

                await _broadcastedTransactionStateRepository.AddOrReplaceAsync(new BroadcastedTransactionStateDto
                {
                    Amount      = operation.Amount,
                    Fee         = new BigInteger(), // TODO: Set fee,
                    FromAddress = operation.FromAddress,
                    OperationId = operationId,
                    State       = TransactionState.InProgress,
                    Timestamp   = now,
                    ToAddress   = operation.ToAddress,
                    TxHash      = txHash
                });

                return txHash;
            }
            else
            {
                throw new ConflictException($"Specified transaction [{signedTxData}] for specified operation [{operationId:N}] has laready been broadcasted.");
            }
        }

        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            var operation = await _builtTransactionRepository.GetAsync(operationId);

            if (operation == null)
            {
                var nonce        = await _ethereum.GetNextNonceAsync(fromAddress);
                var gasPrice     = await _gasPriceOracleService.CalculateGasPriceAsync(toAddress, amount);
                var fee          = gasPrice * Constants.EtcTransferGasAmount;
                var actualAmount = amount;

                if (includeFee)
                {
                    actualAmount -= fee;
                }
                
                await _builtTransactionRepository.AddAsync(new BuiltTransactionDto
                {
                    Amount      = actualAmount,
                    FromAddress = fromAddress,
                    GasPrice    = gasPrice,
                    IncludeFee  = includeFee,
                    Nonce       = nonce,
                    OperationId = operationId,
                    ToAddress   = toAddress
                });

                return _ethereum.BuildTransaction
                (
                    to:        toAddress,
                    amount:    amount,
                    nonce:     nonce,
                    gasPrice:  gasPrice,
                    gasAmount: Constants.EtcTransferGasAmount
                );
            }
            else
            {
                return _ethereum.BuildTransaction
                (
                    to:        operation.ToAddress,
                    amount:    operation.Amount,
                    nonce:     operation.Nonce,
                    gasPrice:  operation.GasPrice,
                    gasAmount: Constants.EtcTransferGasAmount
                );
            }
        }
        
        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            var operation = await _builtTransactionRepository.GetAsync(operationId);
            
            if (operation != null)
            {
                var gasPrice     = ApplyFeeFactor(operation.GasPrice, feeFactor);
                var fee          = gasPrice * Constants.EtcTransferGasAmount;
                var actualAmount = operation.Amount;

                if (operation.IncludeFee)
                {
                    actualAmount -= fee;
                }

                var txData = _ethereum.BuildTransaction
                (
                    to:        operation.ToAddress,
                    amount:    actualAmount,
                    nonce:     operation.Nonce,
                    gasPrice:  gasPrice,
                    gasAmount: Constants.EtcTransferGasAmount
                );

                return txData;
            }
            else
            {
                throw new NotFoundException($"Operation [{operationId:N}] not found.");
            }
        }

        private static BigInteger ApplyFeeFactor(BigInteger gasPrice, decimal feeFactor)
        {
            if (feeFactor == 0)
            {
                return 0;
            }

            var feeFactorBits       = decimal.GetBits(feeFactor);
            var feeFactorMultiplier = new BigInteger(new decimal(feeFactorBits[0], feeFactorBits[1], feeFactorBits[2], false, 0));
            var decimalPlacesNumber = (int)BitConverter.GetBytes(feeFactorBits[3])[2];
            var feeFactorDivider    = new BigInteger(Math.Pow(10, decimalPlacesNumber));
            var newGasPrice         = gasPrice * feeFactorMultiplier / feeFactorDivider;

            if (newGasPrice > gasPrice)
            {
                return newGasPrice;
            }
            else
            {
                return gasPrice + 1;
            }
        }
    }
}
