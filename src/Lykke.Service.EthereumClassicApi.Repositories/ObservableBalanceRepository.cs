using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Microsoft.WindowsAzure.Storage.Table;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class ObservableBalanceRepository : IObservableBalanceRepository
    {
        private readonly INoSQLTableStorage<ObservableBalanceEntity> _table;


        public ObservableBalanceRepository(
            INoSQLTableStorage<ObservableBalanceEntity> table)
        {
            _table = table;
        }


        private static string GetPartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        private static string GetRowKey(string address)
        {
            return address;
        }


        public async Task AddAsync(ObservableBalanceDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.Address);
            entity.RowKey = GetRowKey(dto.Address);

            await _table.InsertAsync(entity);
        }

        public async Task DeleteAsync(string address)
        {
            await _table.DeleteIfExistAsync
            (
                GetPartitionKey(address),
                GetRowKey(address)
            );
        }

        public async Task<bool> ExistsAsync(string address)
        {
            var entity = await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address));

            return entity != null;
        }

        public async Task<ObservableBalanceDto> TryGetAsync(string address)
        {
            return (await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address)))
                .ToDto();
        }

        public async Task<IEnumerable<ObservableBalanceDto>> GetAllAsync()
        {
            var dtos = new List<ObservableBalanceDto>();

            string continuationToken = null;

            do
            {
                IEnumerable<ObservableBalanceEntity> entities;

                (entities, continuationToken) = await _table.GetDataWithContinuationTokenAsync(1000, continuationToken);

                dtos.AddRange(entities.Select(x => x.ToDto()));

            } while (continuationToken != null);
            
            return dtos;
        }

        public async Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken)
        {
            IEnumerable<ObservableBalanceEntity> entities;
            
            var filterCondition = TableQuery.CombineFilters
            (
                TableQuery.GenerateFilterCondition("Amount", QueryComparisons.NotEqual, "0"),
                "AND",
                TableQuery.GenerateFilterConditionForBool("Locked", QueryComparisons.NotEqual, true)
            );

            var query = new TableQuery<ObservableBalanceEntity>().Where(filterCondition);
            
            (entities, continuationToken) = await _table.GetDataWithContinuationTokenAsync(query, take, continuationToken);
            
            return (entities.Select(x => x.ToDto()), continuationToken);
        }

        public async Task UpdateAsync(string address, BigInteger? amount = null, bool? locked = null)
        {
            ObservableBalanceEntity UpdateAction(ObservableBalanceEntity entity)
            {
                if (amount.HasValue)
                {
                    entity.Amount = amount.Value.ToString();
                }

                if (locked.HasValue)
                {
                    entity.Locked = locked.Value;
                }
                
                return entity;
            }
            
            await _table.MergeAsync
            (
                GetPartitionKey(address),
                GetRowKey(address),
                UpdateAction
            );
        }
    }
}
