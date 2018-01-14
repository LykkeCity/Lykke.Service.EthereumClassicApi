using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Services.DTOs;

namespace Lykke.Service.EthereumClassicApi.Services.Interfaces
{
    public interface ITransactionStateService
    {
        Task<TransactionStateDto> GetTransactionStateAsync(string txHash);
    }
}