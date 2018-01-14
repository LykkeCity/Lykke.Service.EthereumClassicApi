using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public class CheckBalances
    {
        private CheckBalances()
        {

        }

        /// <summary>
        ///    The singleton instance of CheckBalances.
        /// </summary>
        public static CheckBalances Instance { get; }
            = new CheckBalances();
    }
}
