using System.ComponentModel;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class BeginBalanceMonitoring
    {
        public BeginBalanceMonitoring(string address)
        {
            Address = address;
        }

        public string Address { get; }
    }
}
