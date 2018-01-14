using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class BalanceChecked
    {
        private BalanceChecked()
        {

        }

        /// <summary>
        ///    The singleton instance of BalanceChecked.
        /// </summary>
        public static BalanceChecked Instance { get; } 
            = new BalanceChecked();
    }
}
