using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class TransactionParamsDto
    {
        public BigInteger Amount { get; set; }

        public BigInteger Fee { get; set; }

        public BigInteger GasPrice { get; set; }
    }
}
