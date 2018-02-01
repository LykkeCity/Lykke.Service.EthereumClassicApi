﻿using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
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

        
        public async Task<bool> DeleteIfExistsAsync(string address)
        {
            return await _table.DeleteIfExistAsync
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
        
        public async Task<IEnumerable<ObservableBalanceEntity>> GetAllAsync()
        {
            return await _table.GetDataAsync(x => true);
        }

        public async Task<(IEnumerable<ObservableBalanceEntity> Balances, string ContinuationToken)> GetAllWithNonZeroAmountAsync(int take, string continuationToken)
        {
            var filterCondition = TableQuery.CombineFilters
            (
                TableQuery.GenerateFilterCondition("Amount", QueryComparisons.NotEqual, "0"),
                TableOperators.And,
                TableQuery.GenerateFilterConditionForBool("Locked", QueryComparisons.NotEqual, true)
            );
            
            var query = new TableQuery<ObservableBalanceEntity>().Where(filterCondition);
            
            return await _table.GetDataWithContinuationTokenAsync(query, take, continuationToken);
        }

        public async Task<bool> TryAddAsync(string address)
        {
            var entity = new ObservableBalanceEntity
            {
                PartitionKey = GetPartitionKey(address),
                RowKey = GetRowKey(address),

                Address = address,
                Amount = 0,
                Locked = false
            };

            return await _table.TryInsertAsync(entity);
        }

        public async Task<ObservableBalanceEntity> TryGetAsync(string address)
        {
            return await _table.GetDataAsync(GetPartitionKey(address), GetRowKey(address));
        }

        public async Task UpdateAmountAsync(string address, BigInteger amount)
        {
            ObservableBalanceEntity UpdateAction(ObservableBalanceEntity entity)
            {
                entity.Amount = amount;

                return entity;
            }

            await _table.MergeAsync
            (
                GetPartitionKey(address),
                GetRowKey(address),
                UpdateAction
            );
        }

        public async Task UpdateLockAsync(string address, bool locked)
        {
            ObservableBalanceEntity UpdateAction(ObservableBalanceEntity entity)
            {
                entity.Amount = 0;
                entity.Locked = locked;

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
