using System;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Exceptions;
using Lykke.Service.EthereumClassicApi.Common.Utils;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Extensions;
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


        public async Task<string> BroadcastTransactionAsync(Guid operationId, string signedTxData)
        {
            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId)).ToList();

            if (operationTransactions.Any(x => x.SignedTxData == signedTxData))
            {
                throw new ConflictException
                (
                    $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has laready been broadcasted."
                );
            }

            var txData = _ethereum.UnsignTransaction(signedTxData);
            var builtTransaction = operationTransactions.FirstOrDefault(x => x.TxData == txData);
            if (builtTransaction == null)
            {
                throw new BadRequestException
                (
                    $"Specified transaction [{signedTxData}] for specified operation [{operationId}] has not been found."
                );
            }

            await LockBalanceIfNecessaryAsync(builtTransaction.FromAddress);

            var txHash = await SendRawTransactionOrGetTxHashAsync(signedTxData);

            await WaitUntilTransactionIsInPoolAsync(txHash);

            await _transactionRepository.UpdateAsync(new BroadcastedTransactionDto
            {
                BroacastedOn = DateTime.UtcNow,
                OperationId = builtTransaction.OperationId,
                SignedTxData = signedTxData,
                SignedTxHash = txHash,
                State = TransactionState.InProgress,
                TxData = builtTransaction.TxData
            });

            return txHash;
        }

        public async Task<string> BuildTransactionAsync(BigInteger amount, BigInteger fee, string fromAddress, BigInteger gasPrice, bool includeFee, Guid operationId, string toAddress)
        {
            #region Validation
            
            if (amount <= 0)
            {
                throw new ArgumentException("Amount should be greater then zero.", nameof(amount));
            }

            if (fee <= 0)
            {
                throw new ArgumentException("Fee should be greater then zero.", nameof(fee));
            }
            
            if (gasPrice <= 0)
            {
                throw new ArgumentException("Gas price should be greater then zero.", nameof(gasPrice));
            }

            if (!await AddressValidator.ValidateAsync(fromAddress))
            {
                throw new ArgumentException("Address is invalid.", nameof(fromAddress));
            }

            if (!await AddressValidator.ValidateAsync(toAddress))
            {
                throw new ArgumentException("Address is invalid.", nameof(toAddress));
            }

            #endregion

            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId));
            var initialTransaction = operationTransactions.OrderBy(x => x.BuiltOn).FirstOrDefault();

            if (initialTransaction != null)
            {
                return initialTransaction.TxData;
            }

            var nonce = await _ethereum.GetNextNonceAsync(fromAddress);
            
            var txData = _ethereum.BuildTransaction
            (
                toAddress,
                amount,
                nonce,
                gasPrice,
                Constants.EtcTransferGasAmount
            );

            await _transactionRepository.AddAsync(new BuiltTransactionDto
            {
                Amount = amount,
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

        public async Task<TransactionParamsDto> CalculateTransactionParamsAsync(BigInteger amount, bool includeFee, string toAddress)
        {
            #region Validation

            if (amount <= 0)
            {
                throw new ArgumentException("Amount should be greater then zero.", nameof(amount));
            }
            
            if (!await AddressValidator.ValidateAsync(toAddress))
            {
                throw new ArgumentException("Address is invalid.", nameof(toAddress));
            }

            #endregion

            var gasPrice = await _gasPriceOracleService.CalculateGasPriceAsync(toAddress, amount);
            var fee = gasPrice * Constants.EtcTransferGasAmount;

            if (includeFee)
            {
                amount -= fee;
            }

            return new TransactionParamsDto
            {
                Amount = amount,
                Fee = fee,
                GasPrice = gasPrice
            };
        }

        public async Task<string> RebuildTransactionAsync(decimal feeFactor, Guid operationId)
        {
            #region Validation

            if (feeFactor <= 1m)
            {
                throw new ArgumentException("Fee factor should be greater then one.", nameof(feeFactor));
            }

            #endregion

            var operationTransactions = (await _transactionRepository.GetAllAsync(operationId)).ToList();
            var initialTransaction    = operationTransactions.OrderBy(x => x.BuiltOn).FirstOrDefault();

            if (initialTransaction == null)
            {
                throw new BadRequestException($"Initial transaction for specified operation [{operationId}] has not been not found.");
            }

            var txParams = initialTransaction.CalculateTransactionParams(feeFactor);
            
            var txData = _ethereum.BuildTransaction
            (
                initialTransaction.ToAddress,
                txParams.Amount,
                initialTransaction.Nonce,
                txParams.GasPrice,
                Constants.EtcTransferGasAmount
            );

            // If same transaction has not been built earlier, persisting it
            if (operationTransactions.All(x => x.TxData != txData))
            {
                await _transactionRepository.AddAsync(new BuiltTransactionDto
                {
                    Amount = txParams.Amount,
                    BuiltOn = DateTime.UtcNow,
                    Fee = txParams.Fee,
                    FromAddress = initialTransaction.FromAddress,
                    GasPrice = txParams.GasPrice,
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

        private async Task WaitUntilTransactionIsInPoolAsync(string txHash)
        {
            var retryCount = 0;

            do
            {

                try
                {
                    var isBroadcasted = await _ethereum.CheckIfBroadcastedAsync(txHash);

                    if (isBroadcasted)
                    {
                        return;
                    }
                    else
                    {
                        await Task.Delay(TimeSpan.FromSeconds(1));
                    }
                }
                catch (Exception)
                {
                    // ignored
                }

            } while (retryCount++ < 5);

            throw new UnsupportedEdgeCaseException("Transaction not appeared in memory pool in the specified period of time.");
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
    }
}
