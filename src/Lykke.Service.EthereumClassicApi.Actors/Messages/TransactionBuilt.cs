using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class TransactionBuilt
    {
        public TransactionBuilt(string txData)
        {
            TxData = txData;
        }


        public string TxData { get; }
    }
}
