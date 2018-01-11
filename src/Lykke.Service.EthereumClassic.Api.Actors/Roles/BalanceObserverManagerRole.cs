using System.Threading.Tasks;
using Lykke.Service.EthereumClassic.Api.Actors.Exceptions;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassic.Api.Repositories.DTOs;
using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors.Roles
{
    public class BalanceObserverManagerRole : IBalanceObserverManagerRole
    {
        private readonly IObservableBalanceRepository _observableBalanceRepository;

        public BalanceObserverManagerRole(
            IObservableBalanceRepository observableBalanceRepository)
        {
            _observableBalanceRepository = observableBalanceRepository;
        }


        public async Task BeginBalanceMonitoringAsync(string address)
        {
            if (!await _observableBalanceRepository.ExistsAsync(address))
            {
                await _observableBalanceRepository.AddAsync(new ObservableBalanceDto
                {
                    Address = address
                });
            }
            else
            {
                throw new NotFoundException($"Specified address [{address}] is already observed.");
            }
        }

        public async Task EndBalanceMonitoringAsync(string address)
        {
            if (await _observableBalanceRepository.ExistsAsync(address))
            {
                await _observableBalanceRepository.DeleteAsync(address);
            }
            else
            {
                throw new NotFoundException($"Specified address [{address}] is not observed.");
            }
        }
    }
}
