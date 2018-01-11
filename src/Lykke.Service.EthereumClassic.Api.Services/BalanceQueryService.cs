using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;
using Lykke.Service.EthereumClassic.Api.Services.DTOs;
using Lykke.Service.EthereumClassic.Api.Services.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Services
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
