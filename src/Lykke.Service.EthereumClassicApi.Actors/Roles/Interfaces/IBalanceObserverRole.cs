using System.Numerics;
using System.Threading.Tasks;

namespace Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces
{
    public interface IBalanceObserverRole : IActorRole
    {
        /// <summary>
        ///    Get the balance of an address at a specified block and store in query-side table.
        /// </summary>
        /// <param name="address">
        ///    The address to get the balance of.
        /// </param>
        /// <param name="blockNumber">
        ///    The block number to get the balance at.
        /// </param>
        /// <returns>
        ///    A BigInteger instance of the current balance for the given address in wei.
        /// </returns>
        Task<BigInteger> GetBalanceAsync(string address, BigInteger blockNumber);
    }
}
