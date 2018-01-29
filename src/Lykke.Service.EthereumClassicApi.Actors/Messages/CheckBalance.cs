using System.ComponentModel;
using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Actors.Messages
{
    [ImmutableObject(true)]
    public sealed class CheckBalance
    {
        public CheckBalance(string address, BigInteger blockNumbber)
        {
            Address = address;
            BlockNumber = blockNumbber;
        }

        public string Address { get; }

        public BigInteger BlockNumber { get; }
    }
}
