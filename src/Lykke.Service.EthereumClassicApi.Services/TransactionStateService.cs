using System;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Services.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class TransactionStateService : ITransactionStateService
    {
        private readonly IEthereum _ethereum;
        private readonly EthereumClassicApiSettings _settings;


        public TransactionStateService(
            IEthereum ethereum,
            EthereumClassicApiSettings settings)
        {
            _ethereum = ethereum;
            _settings = settings;
        }


        public async Task<TransactionStateDto> GetTransactionStateAsync(string txHash)
        {
            var latestBlockNumber = await _ethereum.GetLatestBlockNumberAsync();
            var latestConfirmedBlockNumber = latestBlockNumber - _settings.TransactionConfirmationLevel;
            var receipt = await _ethereum.GetTransactionReceiptAsync(txHash);

            if (receipt?.BlockHash != null && receipt.BlockNumber <= latestConfirmedBlockNumber)
            {
                var transactionError = await _ethereum.GetTransactionErrorAsync(txHash);
                var transactionState = string.IsNullOrEmpty(transactionError) ? TransactionState.Completed : TransactionState.Failed;
                var blockTimestamp = await _ethereum.GetTimestampAsync(receipt.BlockNumber);
                

                return new TransactionStateDto
                {
                    BlockNumber = receipt.BlockNumber,
                    Error = transactionError,
                    State = transactionState,
                    CompletedOn = DateTimeOffset.FromUnixTimeSeconds((long)blockTimestamp).UtcDateTime
                };
            }

            return new TransactionStateDto
            {
                BlockNumber = null,
                Error = null,
                State = TransactionState.InProgress,
                CompletedOn = null
            };
        }
    }
}
