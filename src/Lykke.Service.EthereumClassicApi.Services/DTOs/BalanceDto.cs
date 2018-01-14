using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Services.DTOs
{
    public class BalanceDto
    {
        public string Address { get; set; }

        public BigInteger Balance { get; set; }
    }
}
