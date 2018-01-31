using System;
using System.Linq;
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
    public class TransactionService : ITransactionService
    {
        private readonly IEthereum _ethereum;
        private readonly IGasPriceOracleService _gasPriceOracleService;
        private readonly IObservableBalanceRepository _observableBalanceRepository;
        private readonly ITransactionRepository _transactionRepository;

        public TransactionService(
            IEthereum ethereum,
            IGasPriceOracleService gasPriceOracleService,
            IObservableBalanceRepository observableBalanceRepository,
            ITransactionRepository transactionRepository)
        {
            _ethereum = ethereum;
            _gasPriceOracleService = gasPriceOracleService;
            _observableBalanceRepository = observableBalanceRepository;
            _transactionRepository = transactionRepository;
        }


        private static BigInteger ApplyFeeFactor(BigInteger gasPrice, decimal feeFactor)
        {
            if (feeFactor == 0)
            {
                return 0;
            }

            var feeFactorBits = decimal.GetBits(feeFactor);
            var feeFactorMultiplier =
                new BigInteger(new decimal(feeFactorBits[0], feeFactorBits[1], feeFactorBits[2], false, 0));
            var decimalPlacesNumber = (int) BitConverter.GetBytes(feeFactorBits[3])[2];
            var feeFactorDivider = new BigInteger(Math.Pow(10, decimalPlacesNumber));
            var newGasPrice = gasPrice * feeFactorMultiplier / feeFactorDivider;

            if (newGasPrice > gasPrice)
            {
                return newGasPrice;
            }

            return gasPrice + 1;
        }

        public async Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId))
                .ToList();

            if (operationTransactions.All(x => x.SignedTxData != signedTxData))
            {
                var txData = _ethereum.UnsignTransaction(signedTxData);
                var builtTransaction = operationTransactions.FirstOrDefault(x => x.TxData == txData);
                if (builtTransaction == null)
                {
                    throw new NotFoundException
                    (
                        $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has not been found."
                    );
                }

                await LockBalanceIfNecessaryAsync(builtTransaction.FromAddress);

                var txHash = await SendRawTransactionOrGetTxHashAsync(signedTxData);
                var now = DateTime.UtcNow;

                await _transactionRepository.UpdateAsync(new BroadcastedTransactionDto
                {
                    BroacastedOn = now,
                    OperationId = builtTransaction.OperationId,
                    SignedTxData = signedTxData,
                    SignedTxHash = txHash,
                    State = TransactionState.InProgress,
                    TxData = builtTransaction.TxData
                });

                return txHash;
            }

            throw new ConflictException
            (
                $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has laready been broadcasted."
            );
        }

        private async Task LockBalanceIfNecessaryAsync(string fromAddress)
        {
            if (await _observableBalanceRepository.ExistsAsync(fromAddress))
            {
                await _observableBalanceRepository.UpdateLockAsync(fromAddress, true);
            }
        }

        /// <summary>
        ///     Sends raw transaction, or, if it has already been sent, returns it's txHash
        /// </summary>
        /// <param name="signedTxData">
        ///     Signed transaction data.
        /// </param>
        /// <returns>
        ///     Transaction hash.
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

        public async Task<string> BuildTransactionAsync(BigInteger amount, string fromAddress, bool includeFee, Guid operationId, string toAddress)
        {
            var initialTransaction = (await _transactionRepository.GetAllAsync(operationId))
                .OrderBy(x => x.BuiltOn)
                .FirstOrDefault();

            if (initialTransaction == null)
            {
                var nonce = await _ethereum.GetNextNonceAsync(fromAddress);
                var gasPrice = await _gasPriceOracleService.CalculateGasPriceAsync(toAddress, amount);
                var fee = gasPrice * Constants.EtcTransferGasAmount;
                var actualAmount = amount;

                if (includeFee)
                {
                    actualAmount -= fee;
                }

                if (actualAmount <= 0)
                {
                    throw new BadRequestException("Transaction amount is too low.");
                }

                var txData = _ethereum.BuildTransaction
                (
                    toAddress,
                    actualAmount,
                    nonce,
                    gasPrice,
                    Constants.EtcTransferGasAmount
                );

                await _transactionRepository.AddAsync(new BuiltTransactionDto
                {
                    Amount = actualAmount,
                    BuiltOn = DateTime.UtcNow,
                    Fee = fee,
                    FromAddress = fromAddress,
                    GasPrice = gasPrice,
                    IncludeFee = includeFee,
                    Nonce = nonce,
                    OperationId = operationId,
                    State = TransactionState.Built,
                    ToAddress = toAddress,
                    TxData = txData
                });

                return txData;
            }

            return _ethereum.BuildTransaction
            (
                initialTransaction.ToAddress,
                initialTransaction.Amount,
                initialTransaction.Nonce,
                initialTransaction.GasPrice,
                Constants.EtcTransferGasAmount
            );
        }

        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId))
                .OrderBy(x => x.BuiltOn)
                .ToList();

            var initialTransaction = operationTransactions.FirstOrDefault();

            if (initialTransaction != null)
            {
                var gasPrice = ApplyFeeFactor(initialTransaction.GasPrice, feeFactor);
                var fee = gasPrice * Constants.EtcTransferGasAmount;
                var actualAmount = initialTransaction.Amount;

                if (initialTransaction.IncludeFee)
                {
                    actualAmount -= fee;
                }

                var txData = _ethereum.BuildTransaction
                (
                    initialTransaction.ToAddress,
                    actualAmount,
                    initialTransaction.Nonce,
                    gasPrice,
                    Constants.EtcTransferGasAmount
                );

                if (operationTransactions.All(x => x.TxData != txData))
                {
                    await _transactionRepository.AddAsync(new BuiltTransactionDto
                    {
                        Amount = actualAmount,
                        BuiltOn = DateTime.UtcNow,
                        FromAddress = initialTransaction.FromAddress,
                        GasPrice = gasPrice,
                        IncludeFee = initialTransaction.IncludeFee,
                        Nonce = initialTransaction.Nonce,
                        OperationId = operationId,
                        State = TransactionState.Built,
                        ToAddress = initialTransaction.ToAddress,
                        TxData = txData
                    });
                }

                return txData;
            }

            throw new NotFoundException($"Initial transaction for specified operation [{operationId}] has not been not found.");
        }
    }
}
