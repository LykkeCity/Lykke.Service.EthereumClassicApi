using System.Threading.Tasks;
using Lykke.Service.EthereumClassicApi.Actors.Exceptions;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.DTOs;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles
{
    public class BalanceObserverManagerRole : IBalanceObserverManagerRole
    {
        private readonly IObservableBalanceRepository _observableBalanceRepository;

        public BalanceObserverManagerRole(
            IObservableBalanceRepository observableBalanceRepository)
        {
            _observableBalanceRepository = observableBalanceRepository;
        }


        /// <inheritdoc />
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
                throw new ConflictException($"Specified address [{address}] is already observed.");
            }
        }

        /// <inheritdoc />
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
