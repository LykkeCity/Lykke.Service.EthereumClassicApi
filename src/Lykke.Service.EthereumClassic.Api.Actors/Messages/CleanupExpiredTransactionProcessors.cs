using System.ComponentModel;

namespace Lykke.Service.EthereumClassic.Api.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CleanupExpiredTransactionProcessors
    {
        private CleanupExpiredTransactionProcessors()
        {

        }

        /// <summary>
        ///    The singleton instance of CleanupExpiredTransactionProcessors.
        /// </summary>
        public static CleanupExpiredTransactionProcessors Instance { get; }
            = new CleanupExpiredTransactionProcessors();
    }
}
