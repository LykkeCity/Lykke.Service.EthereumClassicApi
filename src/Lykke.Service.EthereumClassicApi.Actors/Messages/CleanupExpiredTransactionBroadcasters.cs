using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CleanupExpiredTransactionBroadcasters
    {
        private CleanupExpiredTransactionBroadcasters()
        {
        }

        /// <summary>
        ///     The singleton instance of CleanupExpiredTransactionBroadcasters.
        /// </summary>
        public static CleanupExpiredTransactionBroadcasters Instance { get; }
            = new CleanupExpiredTransactionBroadcasters();
    }
}
