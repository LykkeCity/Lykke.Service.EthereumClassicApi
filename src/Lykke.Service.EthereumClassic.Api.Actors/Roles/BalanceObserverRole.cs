using System.Numerics;
using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassic.Api.Blockchain.Interfaces;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;


namespace Lykke.Service.EthereumClassic.Api.Actors.Roles
{
    public class BalanceObserverRole : IBalanceObserverRole
    {
        private readonly IBalanceRepository _balanceRepository;
        private readonly IEthereum          _ethereum;


        public BalanceObserverRole(
            IBalanceRepository balanceRepository,
            IEthereum ethereum)
        {
            _balanceRepository = balanceRepository;
            _ethereum          = ethereum;
        }


        public async Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber)
        {
            var balanceDto = new BalanceDto
            {
                Address = address,
                Balance = await _ethereum.GetBalanceAsync(address, blockNumber)
            };
            
            await _balanceRepository.AddOrReplaceAsync(balanceDto);

            return balanceDto.Balance;
        }
    }
}
