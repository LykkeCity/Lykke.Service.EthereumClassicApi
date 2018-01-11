using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;
using Lykke.Service.EthereumClassic.Api.Services.DTOs;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Services
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
            var receipt = await _ethereum.GetTransactionReceipt(txHash);

            if (receipt?.BlockHash != null)
            {
                return new TransactionStateDto
                {
                    Completed = true,
                    Failed    = receipt.Status == 0
                };
            }
            else
            {
                return new TransactionStateDto
                {
                    Completed = false,
                    Failed    = false
                };
            }
        }
    }
}