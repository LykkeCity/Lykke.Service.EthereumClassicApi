using System.Numerics;
using Lykke.Service.EthereumClassicApi.Common;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class TransactionStateDto
    {
        public string Error { get; set; }
        
        public BigInteger? Fee { get; set; }

        public TransactionState State { get; set; }
    }
}
