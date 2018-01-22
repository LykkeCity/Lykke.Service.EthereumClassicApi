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
        private readonly IEthereum                  _ethereum;
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
            var latestBlockNumber          = await _ethereum.GetLatestBlockNumberAsync();
            var latestConfirmedBlockNumber = latestBlockNumber - _settings.TransactionConfirmationLevel;
            var receipt                    = await _ethereum.GetTransactionReceiptAsync(txHash);

            if (receipt?.BlockHash != null && receipt.BlockNumber <= latestConfirmedBlockNumber)
            {
                return new TransactionStateDto
                {
                    State = receipt.Status == 0 ? TransactionState.Failed : TransactionState.Completed
                };
            }
            else
            {
                return new TransactionStateDto
                {
                    State = TransactionState.InProgress
                };
            }
        }
    }
}
