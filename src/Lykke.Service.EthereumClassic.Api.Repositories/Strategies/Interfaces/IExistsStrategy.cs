using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces
{
    public interface IExistsStrategy<in T>
        where T : AzureTableEntity, new()
    {
        Task<bool> ExecuteAsync(string partitionKey, string rowKey);
    }
}
