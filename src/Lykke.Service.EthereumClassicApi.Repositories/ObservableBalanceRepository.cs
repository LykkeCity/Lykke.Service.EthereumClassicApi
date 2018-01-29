using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Mappins;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class ObservableBalanceRepository : IObservableBalanceRepository
    {
        private readonly IAddStrategy<ObservableBalanceEntity> _addStrategy;
        private readonly IDeleteStrategy<ObservableBalanceEntity> _deleteStrategy;
        private readonly IExistsStrategy<ObservableBalanceEntity> _existsStrategy;
        private readonly IGetAllStrategy<ObservableBalanceEntity> _getAllStrategy;
        private readonly IReplaceStrategy<ObservableBalanceEntity> _replaceStrategy;


        public ObservableBalanceRepository(
            IAddStrategy<ObservableBalanceEntity> addStrategy,
            IDeleteStrategy<ObservableBalanceEntity> deleteStrategy,
            IExistsStrategy<ObservableBalanceEntity> existsStrategy,
            IGetAllStrategy<ObservableBalanceEntity> getAllStrategy,
            IReplaceStrategy<ObservableBalanceEntity> replaceStrategy)
        {
            _addStrategy = addStrategy;
            _deleteStrategy = deleteStrategy;
            _existsStrategy = existsStrategy;
            _getAllStrategy = getAllStrategy;
            _replaceStrategy = replaceStrategy;
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

            await _addStrategy.ExecuteAsync(entity);
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

        public async Task<IEnumerable<ObservableBalanceDto>> GetAllAsync()
        {
            return (await _getAllStrategy.ExecuteAsync())
                .Select(x => x.ToDto());
        }

        public async Task<(IEnumerable<ObservableBalanceDto> Balances, string ContinuationToken)>
            GetAllWithNonZeroAmountAsync(int take, string continuationToken)
        {
            IEnumerable<ObservableBalanceEntity> entities;

            (entities, continuationToken) =
                await _getAllStrategy.ExecuteAsync(x => x.Amount != "0", take, continuationToken);

            return (entities.Select(x => x.ToDto()), continuationToken);
        }

        public async Task ReplaceAsync(ObservableBalanceDto dto)
        {
            var entity = dto.ToEntity();

            entity.PartitionKey = GetPartitionKey(dto.Address);
            entity.RowKey = GetRowKey(dto.Address);
            entity.ETag = "*";

            await _replaceStrategy.ExecuteAsync(entity);
        }
    }
}
