using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Services.DTOs;

namespace Lykke.Service.EthereumClassic.Api.Services.Interfaces
{
    public interface ITransactionStateService
    {
        Task<TransactionStateDto> GetTransactionStateAsync(string txHash);
    }
}