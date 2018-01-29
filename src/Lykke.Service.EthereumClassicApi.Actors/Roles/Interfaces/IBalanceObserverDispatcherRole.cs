using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface IBalanceObserverDispatcherRole : IActorRole
    {
        /// <summary>
        ///     Get the latest block number, taking into account a confirmation level.
        /// </summary>
        /// <returns>
        ///     The latest bblock number with substracted cashin confirmation level.
        /// </returns>
        Task<BigInteger> GetLatestConfirmedBlockNumber();

        /// <summary>
        ///     Get the list of balance holders.
        /// </summary>
        /// <returns>
        ///     All of the wallet addresses that used for cashin operations.
        /// </returns>
        Task<IEnumerable<string>> GetObservableAddressesAsync();
    }
}
