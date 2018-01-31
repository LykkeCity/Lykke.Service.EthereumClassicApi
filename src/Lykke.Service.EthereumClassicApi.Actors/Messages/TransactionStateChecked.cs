using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class TransactionStateChecked
    {
        private TransactionStateChecked()
        {
        }

        /// <summary>
        ///     The singleton instance of TransactionStateChecked.
        /// </summary>
        public static TransactionStateChecked Instance { get; }
            = new TransactionStateChecked();
    }
}
