using System.Numerics;

namespace Lykke.Service.EthereumClassic.Api.Services.DTOs
{
    public class BalanceDto
    {
        public string Address { get; set; }

        public BigInteger Balance { get; set; }
    }
}
