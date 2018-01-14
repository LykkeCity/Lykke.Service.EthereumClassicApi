using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces
{
    public interface IExistsStrategy<in T>
        where T : AzureTableEntity, new()
    {
        Task<bool> ExecuteAsync(string partitionKey, string rowKey);
    }
}
