using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Actors.Exceptions;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;
using Lykke.Service.EthereumClassic.Api.Common;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;


namespace Lykke.Service.EthereumClassic.Api.Actors.Roles
{
    public class TransactionProcessorRole : ITransactionProcessorRole
    {
        private readonly IEthereum                       _ethereum;
        private readonly IGasPriceOracleService          _gasPriceOracleService;
        private readonly IOperationRepository            _operationRepository;
        private readonly IOperationTransactionRepository _operationTransactionRepository;


        public TransactionProcessorRole(
            IEthereum ethereum,
            IGasPriceOracleService gasPriceOracleService,
            IOperationRepository operationRepository,
            IOperationTransactionRepository operationTransactionRepository)
        {
            _ethereum                       = ethereum;
            _gasPriceOracleService          = gasPriceOracleService;
            _operationRepository            = operationRepository;
            _operationTransactionRepository = operationTransactionRepository;
        }

        /// <inheritdoc />
        public async Task<string> BroadcastTransaction(Guid operationId, string signedTxData)
        {
            if (!await _operationTransactionRepository.ExistsAsync(operationId, signedTxData))
            {
                var operation = await _operationRepository.GetAsync(operationId);
                var txHash    = await _ethereum.SendRawTransactionAsync(signedTxData);

                await _operationTransactionRepository.AddAsync(new OperationTransactionDto
                {
                    Amount       = operation.Amount,
                    Fee          = new BigInteger(), // TODO: Set fee
                    FromAddress  = operation.FromAddress,
                    OperationId  = operationId,
                    SignedTxData = signedTxData,
                    CreatedOn    = DateTimeOffset.UtcNow,
                    ToAddress    = operation.ToAddress,
                    TxHash       = txHash
                });

                return txHash;
            }
            else
            {
                throw new ConflictException($"Specified transaction [{signedTxData}] for specified operation [{operationId:N}] has laready been broadcasted.");
            }
        }

        public async Task<string> BuildOperationAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            var operation = await _operationRepository.GetAsync(operationId);

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
                
                await _operationRepository.AddAsync(new OperationDto
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
            var operation = await _operationRepository.GetAsync(operationId);
            
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
