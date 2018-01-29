using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces
{
    public interface IGetAllStrategy<T>
        where T : AzureTableEntity, new()
    {
        Task<IEnumerable<T>> ExecuteAsync();

        Task<(IEnumerable<T> Entities, string ContinuationToken)> ExecuteAsync(int task, string continuationToken);

        Task<IEnumerable<T>> ExecuteAsync(Func<T, bool> filter);

        Task<(IEnumerable<T> Entities, string ContinuationToken)> ExecuteAsync(Func<T, bool> filter, int task,
            string continuationToken);

        Task<IEnumerable<T>> ExecuteAsync(string partitionKey);

        Task<(IEnumerable<T> Entities, string ContinuationToken)> ExecuteAsync(string partitionKey, int task,
            string continuationToken);
    }
}
