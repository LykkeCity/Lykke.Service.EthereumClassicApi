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
    public class BalanceRepository : IBalanceRepository
    {
        private readonly IAddOrReplaceStrategy<BalanceEntity> _addOrReplaceStrategy;
        private readonly IDeleteStrategy<BalanceEntity>       _deleteStrategy;
        private readonly IGetAllStrategy<BalanceEntity>       _getAllStrategy;


        public BalanceRepository(
            IAddOrReplaceStrategy<BalanceEntity> addOrReplaceStrategy,
            IDeleteStrategy<BalanceEntity> deleteStrategy,
            IGetAllStrategy<BalanceEntity> getAllStrategy)
        {
            _addOrReplaceStrategy = addOrReplaceStrategy;
            _deleteStrategy       = deleteStrategy;
            _getAllStrategy       = getAllStrategy;
        }


        public async Task AddOrReplaceAsync(BalanceDto dto)
        {
            var entity = dto.ToEntity();
            
            entity.PartitionKey = GetPartitionKey();
            entity.RowKey       = GetRowKey(dto.Address);

            await _addOrReplaceStrategy.ExecuteAsync(entity);
        }

        public async Task DeleteAsync(string address)
        {
            await _deleteStrategy.ExecuteAsync(GetPartitionKey(), GetRowKey(address));
        }

        public async Task<(IEnumerable<BalanceDto> Balances, string ContinuationToken)> GetAsync(int take, string continuationToken)
        {
            var balances = (await _getAllStrategy.ExecuteAsync(GetPartitionKey()))
                .Select(x => x.ToDto());

            return (balances, null);
        }


        // TODO: Add segmentation
        private static string GetPartitionKey()
            => "Balance";

        private static string GetRowKey(string address)
            => address;
    }
}
