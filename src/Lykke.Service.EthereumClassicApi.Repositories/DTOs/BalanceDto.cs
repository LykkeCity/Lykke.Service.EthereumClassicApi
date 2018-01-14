using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class BalanceDto
    {
        public string Address { get; set; }

        public BigInteger Balance { get; set; }
    }
}
