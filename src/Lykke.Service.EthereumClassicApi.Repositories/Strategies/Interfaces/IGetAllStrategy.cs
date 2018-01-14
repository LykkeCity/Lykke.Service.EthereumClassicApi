using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces
{
    public interface IGetAllStrategy<T>
        where T : AzureTableEntity, new()
    {
        Task<IEnumerable<T>> ExecuteAsync(string partitionKey);
    }
}
