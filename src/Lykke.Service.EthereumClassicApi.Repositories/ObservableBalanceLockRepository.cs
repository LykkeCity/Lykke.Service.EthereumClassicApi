using System.Threading.Tasks;
using AzureStorage;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;


namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class ObservableBalanceLockRepository : IObservableBalanceLockRepository
    {
        private readonly INoSQLTableStorage<ObservableBalanceLockEntity> _table;


        public ObservableBalanceLockRepository(
            INoSQLTableStorage<ObservableBalanceLockEntity> table)
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


        public async Task AddOrReplaceAsync(ObservableBalanceLockDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.Address);
            entity.RowKey = GetRowKey(dto.Address);

            await _table.InsertOrReplaceAsync(entity);
        }

        public async Task DeleteIfExistsAsync(string address)
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
    }
}
