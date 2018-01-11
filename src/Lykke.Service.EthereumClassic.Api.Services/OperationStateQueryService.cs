using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Services.DTOs;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;




namespace Lykke.Service.EthereumClassic.Api.Services
{
    public class OperationStateQueryService : IOperationStateQueryService
    {
        private readonly IOperationStateRepository _operationStateRepository;


        public OperationStateQueryService(
            IOperationStateRepository operationStateRepository)
        {
            _operationStateRepository = operationStateRepository;
        }


        public async Task<IEnumerable<OperationStateDto>> GetCompletedOperationsAsync(int take, string continuationToken)
            => await GetOperationsAsync(true, false, take, continuationToken);

        public async Task<IEnumerable<OperationStateDto>> GetFailedOperationsAsync(int take, string continuationToken)
            => await GetOperationsAsync(true, true, take, continuationToken);

        public async Task<IEnumerable<OperationStateDto>> GetInProgressOperationsAsync(int take, string continuationToken)
            => await GetOperationsAsync(false, false, take, continuationToken);

        private async Task<IEnumerable<OperationStateDto>> GetOperationsAsync(bool completed, bool failed, int take, string continuationToken)
        {
            return (await _operationStateRepository.GetAsync(completed, failed, take, continuationToken))
                .Select(x => new OperationStateDto
                {
                    Amount      = x.Amount,
                    FromAddress = x.FromAddress,
                    OperationId = x.OperationId,
                    Timestamp   = x.Timestamp,
                    ToAddress   = x.ToAddress,
                    TxHash      = x.TxHash
                });
        }
    }
}
