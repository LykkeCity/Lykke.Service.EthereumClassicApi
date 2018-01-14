using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Services.DTOs;
using Lykke.Service.EthereumClassicApi.Services.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Services
{
    public class BalanceQueryService : IBalanceQueryService
    {
        private readonly IBalanceRepository _balanceRepository;


        public BalanceQueryService(
            IBalanceRepository balanceRepository)
        {
            _balanceRepository = balanceRepository;
        }

        
        public async Task<(IEnumerable<BalanceDto>, string)> GetBalancesAsync(int take, string continuationToken)
        {
            var balances = (await _balanceRepository.GetAsync(take, continuationToken))
                .Select(x => new BalanceDto
                {
                    Address = x.Address,
                    Balance = x.Balance
                });

            return (balances, null);
        }
    }
}
