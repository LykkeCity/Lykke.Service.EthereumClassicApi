using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories
{
    public class OperationTransactionRepository : IOperationTransactionRepository
    {
        public async Task AddAsync(OperationTransactionDto dto)
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

        public async Task<IEnumerable<OperationTransactionDto>> GetAsync(Guid operationId)
        {
            throw new NotImplementedException();
        }
    }
}
