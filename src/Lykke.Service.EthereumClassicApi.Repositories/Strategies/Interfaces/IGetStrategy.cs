using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces
{
    public interface IGetStrategy<T>
        where T : AzureTableEntity, new()
    {
        Task<T> ExecuteAsync(string partitionKey, string rowKey);
    }
}
