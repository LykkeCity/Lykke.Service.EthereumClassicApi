using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Entities;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Repositories.Mappins;
using Lykke.Service.EthereumClassic.Api.Repositories.Strategies.Interfaces;


namespace Lykke.Service.EthereumClassic.Api.Repositories
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

        public async Task<IEnumerable<BalanceDto>> GetAsync(int take, string continuationToken)
        {
            return (await _getAllStrategy.ExecuteAsync(GetPartitionKey()))
                .Select(x => x.ToDto());
        }


        private static string GetPartitionKey()
            => "Balance";

        private static string GetRowKey(string address)
            => address;
    }
}
