using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class ObservableBalanceRepository : IObservableBalanceRepository
    {
        private readonly IAddStrategy<ObservableBalanceEntity>    _addStrategy;
        private readonly IDeleteStrategy<ObservableBalanceEntity> _deleteStrategy;
        private readonly IExistsStrategy<ObservableBalanceEntity> _existsStrategy;
        private readonly IGetAllStrategy<ObservableBalanceEntity> _getAllStrategy;


        public ObservableBalanceRepository(
            IAddStrategy<ObservableBalanceEntity> addStrategy,
            IDeleteStrategy<ObservableBalanceEntity> deleteStrategy,
            IExistsStrategy<ObservableBalanceEntity> existsStrategy,
            IGetAllStrategy<ObservableBalanceEntity> getAllStrategy)
        {
            _addStrategy    = addStrategy;
            _deleteStrategy = deleteStrategy;
            _existsStrategy = existsStrategy;
            _getAllStrategy = getAllStrategy;
        }


        public async Task AddAsync(ObservableBalanceDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey();
            entity.RowKey       = GetRowKey(dto.Address);

            await _addStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(string address)
        {
            await _deleteStrategy.ExecuteAsync(GetPartitionKey(), GetRowKey(address));
        }

        public async Task<bool> ExistsAsync(string address)
        {
            return await _existsStrategy.ExecuteAsync(GetPartitionKey(), GetRowKey(address));
        }

        public async Task<IEnumerable<ObservableBalanceDto>> GetAllAsync()
        {
            return (await _getAllStrategy.ExecuteAsync(GetPartitionKey()))
                .Select(x => x.ToDto());
        }

        private static string GetPartitionKey()
            => "ObservableBalance";

        private static string GetRowKey(string address)
            => address;
    }
}
