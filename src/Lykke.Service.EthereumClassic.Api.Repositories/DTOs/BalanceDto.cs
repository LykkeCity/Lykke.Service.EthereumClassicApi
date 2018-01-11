using System.Numerics;

namespace Lykke.Service.EthereumClassic.Api.Repositories.DTOs
{
    public class BalanceDto
    {
        public string Address { get; set; }

        public BigInteger Balance { get; set; }
    }
}
