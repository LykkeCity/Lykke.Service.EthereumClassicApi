using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public class CheckTransactionStates
    {
        private CheckTransactionStates()
        {
        }

        /// <summary>
        ///     The singleton instance of CheckTransactionStates.
        /// </summary>
        public static CheckTransactionStates Instance { get; }
            = new CheckTransactionStates();
    }
}
