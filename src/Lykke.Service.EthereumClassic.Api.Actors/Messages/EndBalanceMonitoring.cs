using System.ComponentModel;

namespace Lykke.Service.EthereumClassic.Api.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class EndBalanceMonitoring
    {
        public EndBalanceMonitoring(string address)
        {
            Address = address;
        }


        public string Address { get; }
    }
}
