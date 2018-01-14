using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface IBalanceObserverManagerRole : IActorRole
    {
        /// <summary>
        ///    Adds specified address to the balance observation list.
        /// </summary>
        /// <param name="address">
        ///    The address to add to balance observation list.
        /// </param>
        /// <exception cref="Exceptions.ConflictException">
        ///    Thrown when specified address is already presented in the balance observation list.
        /// </exception>
        Task BeginBalanceMonitoringAsync(string address);

        /// <summary>
        ///    Removes specified address from the balance observation list.
        /// </summary>
        /// <param name="address">
        ///    The address to add to balance observation list.
        /// </param>
        /// <exception cref="Exceptions.NotFoundException">
        ///    Thrown when specified address is not presented in the balance observation list.
        /// </exception>
        Task EndBalanceMonitoringAsync(string address);
    }
}
