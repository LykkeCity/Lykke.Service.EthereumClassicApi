using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;


namespace Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces
{
    public interface IGetStrategy<T>
        where T : AzureTableEntity, new()
    {
        Task<T> ExecuteAsync(string partitionKey, string rowKey);

        Task<IEnumerable<T>> ExecuteAsync(string partitionKey, int take, int skip);
    }
}
