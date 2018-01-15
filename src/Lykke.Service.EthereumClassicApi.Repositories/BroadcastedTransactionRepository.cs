using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class BroadcastedTransactionRepository : IBroadcastedTransactionRepository
    {
        public async Task AddAsync(BroadcastedTransactionDto dto)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(Guid operationId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ExistsAsync(Guid operation, string signedTxData)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<BroadcastedTransactionDto>> GetAsync(Guid operationId)
        {
            throw new NotImplementedException();
        }
    }
}
