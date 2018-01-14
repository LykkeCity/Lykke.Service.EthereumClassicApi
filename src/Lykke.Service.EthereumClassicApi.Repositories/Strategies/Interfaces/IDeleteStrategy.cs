using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces
{
    public interface IDeleteStrategy<in T>
        where T : AzureTableEntity, new()
    {
        Task ExecuteAsync(string partitionKey, string rowKey);
    }
}
