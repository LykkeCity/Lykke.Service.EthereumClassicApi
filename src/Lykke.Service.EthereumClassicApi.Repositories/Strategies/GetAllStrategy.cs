using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorage;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Strategies
{
    public class GetAllStrategy<T> : IGetAllStrategy<T>
        where T : AzureTableEntity, new()
    {
        private readonly INoSQLTableStorage<T> _table;


        public GetAllStrategy(
            INoSQLTableStorage<T> table)
        {
            _table = table;
        }


        public async Task<IEnumerable<T>> ExecuteAsync()
        {
            var    entities          = new List<T>();
            string continuationToken = null;

            do
            {

                var page = await ExecuteAsync(1000, continuationToken);

                entities.AddRange(page.Entities);

                continuationToken = page.ContinuationToken;

            } while (continuationToken != null);

            return entities;
        }

        public async Task<(IEnumerable<T> Entities, string ContinuationToken)> ExecuteAsync(int take, string continuationToken)
        {
            //TODO: Add pagination

            var entities = await _table.GetDataAsync();
            
            return (entities, null);
        }

        public async Task<IEnumerable<T>> ExecuteAsync(Func<T, bool> filter)
        {
            var    entities          = new List<T>();
            string continuationToken = null;

            do
            {

                var page = await ExecuteAsync(filter, 1000, continuationToken);

                entities.AddRange(page.Entities);

                continuationToken = page.ContinuationToken;

            } while (continuationToken != null);

            return entities;
        }

        public async Task<(IEnumerable<T> Entities, string ContinuationToken)> ExecuteAsync(Func<T, bool> filter, int task, string continuationToken)
        {
            //TODO: Add pagination

            var entities = await _table.GetDataAsync(filter);

            return (entities, null);
        }

        public async Task<IEnumerable<T>> ExecuteAsync(string partitionKey)
        {
            var    entities          = new List<T>();
            string continuationToken = null;

            do
            {

                var page = await ExecuteAsync(partitionKey, 1000, continuationToken);

                entities.AddRange(page.Entities);

                continuationToken = page.ContinuationToken;

            } while (continuationToken != null);

            return entities;
        }

        public async Task<(IEnumerable<T> Entities, string ContinuationToken)> ExecuteAsync(string partitionKey, int take, string continuationToken)
        {
            //TODO: Add pagination

            var entities = await _table.GetDataAsync(partitionKey);

            return (entities, null);
        }
    }
}
