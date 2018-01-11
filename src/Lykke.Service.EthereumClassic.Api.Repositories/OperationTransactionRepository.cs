using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories
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
