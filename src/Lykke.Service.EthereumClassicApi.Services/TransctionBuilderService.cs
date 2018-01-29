using System;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class TransctionBuilderService : ITransactionBuilderService
    {
        private readonly IBuiltTransactionRepository _builtTransactionRepository;
        private readonly IEthereum _ethereum;
        private readonly IGasPriceOracleService _gasPriceOracleService;

        public TransctionBuilderService(
            IBuiltTransactionRepository builtTransactionRepository,
            IEthereum ethereum,
            IGasPriceOracleService gasPriceOracleService)
        {
            _builtTransactionRepository = builtTransactionRepository;
            _ethereum = ethereum;
            _gasPriceOracleService = gasPriceOracleService;
        }


        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            var operation = await _builtTransactionRepository.TryGetAsync(operationId);

            if (operation == null)
            {
                var nonce = await _ethereum.GetNextNonceAsync(fromAddress);
                var gasPrice = await _gasPriceOracleService.CalculateGasPriceAsync(toAddress, amount);
                var fee = gasPrice * Constants.EtcTransferGasAmount;
                var actualAmount = amount;

                if (includeFee)
                {
                    actualAmount -= fee;
                }

                await _builtTransactionRepository.AddAsync(new BuiltTransactionDto
                {
                    Amount = actualAmount,
                    FromAddress = fromAddress,
                    GasPrice = gasPrice,
                    IncludeFee = includeFee,
                    Nonce = nonce,
                    OperationId = operationId,
                    ToAddress = toAddress
                });

                return _ethereum.BuildTransaction
                (
                    toAddress,
                    actualAmount,
                    nonce,
                    gasPrice,
                    Constants.EtcTransferGasAmount
                );
            }

            return _ethereum.BuildTransaction
            (
                operation.ToAddress,
                operation.Amount,
                operation.Nonce,
                operation.GasPrice,
                Constants.EtcTransferGasAmount
            );
        }

        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            var operation = await _builtTransactionRepository.TryGetAsync(operationId);

            if (operation != null)
            {
                var gasPrice = ApplyFeeFactor(operation.GasPrice, feeFactor);
                var fee = gasPrice * Constants.EtcTransferGasAmount;
                var actualAmount = operation.Amount;

                if (operation.IncludeFee)
                {
                    actualAmount -= fee;
                }

                var txData = _ethereum.BuildTransaction
                (
                    operation.ToAddress,
                    actualAmount,
                    operation.Nonce,
                    gasPrice,
                    Constants.EtcTransferGasAmount
                );

                return txData;
            }

            throw new NotFoundException($"Operation [{operationId}] not found.");
        }

        private static BigInteger ApplyFeeFactor(BigInteger gasPrice, decimal feeFactor)
        {
            if (feeFactor == 0)
            {
                return 0;
            }

            var feeFactorBits = decimal.GetBits(feeFactor);
            var feeFactorMultiplier = new BigInteger(new decimal(feeFactorBits[0], feeFactorBits[1], feeFactorBits[2], false, 0));
            var decimalPlacesNumber = (int)BitConverter.GetBytes(feeFactorBits[3])[2];
            var feeFactorDivider = new BigInteger(Math.Pow(10, decimalPlacesNumber));
            var newGasPrice = gasPrice * feeFactorMultiplier / feeFactorDivider;

            if (newGasPrice > gasPrice)
            {
                return newGasPrice;
            }

            return gasPrice + 1;
        }
    }
}
