using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Blockchain.Interfaces;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Services.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class TransactionStateService : ITransactionStateService
    {
        private readonly IEthereum _ethereum;


        public TransactionStateService(
            IEthereum ethereum)
        {
            _ethereum = ethereum;
        }


        public async Task<TransactionStateDto> GetTransactionStateAsync(string txHash)
        {
            var receipt = await _ethereum.GetTransactionReceiptAsync(txHash);

            if (receipt?.BlockHash != null)
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
