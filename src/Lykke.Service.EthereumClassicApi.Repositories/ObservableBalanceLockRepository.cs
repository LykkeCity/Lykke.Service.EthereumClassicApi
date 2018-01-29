using System.Threading.Tasks;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class ObservableBalanceLockRepository : IObservableBalanceLockRepository
    {
        private readonly IAddOrReplaceStrategy<ObservableBalanceLockEntity> _addOrReplaceStrategy;
        private readonly IDeleteStrategy<ObservableBalanceLockEntity> _deleteStrategy;
        private readonly IExistsStrategy<ObservableBalanceLockEntity> _existsStrategy;


        public ObservableBalanceLockRepository(
            IAddOrReplaceStrategy<ObservableBalanceLockEntity> addOrReplaceStrategy,
            IDeleteStrategy<ObservableBalanceLockEntity> deleteStrategy,
            IExistsStrategy<ObservableBalanceLockEntity> existsStrategy)
        {
            _addOrReplaceStrategy = addOrReplaceStrategy;
            _deleteStrategy = deleteStrategy;
            _existsStrategy = existsStrategy;
        }


        private static string GetPartitionKey(string address)
        {
            return address.CalculateHexHash32(3);
        }

        private static string GetRowKey(string address)
        {
            return address;
        }


        public async Task AddAsync(ObservableBalanceLockDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.Address);
            entity.RowKey = GetRowKey(dto.Address);

            await _addOrReplaceStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(string address)
        {
            await _deleteStrategy.ExecuteAsync
            (
                GetPartitionKey(address),
                GetRowKey(address)
            );
        }

        public async Task<bool> ExistsAsync(string address)
        {
            return await _existsStrategy.ExecuteAsync
            (
                GetPartitionKey(address),
                GetRowKey(address)
            );
        }
    }
}
