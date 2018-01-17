using System.Numerics;

namespace Lykke.Service.EthereumClassicApi.Repositories.DTOs
{
    public class ObservableBalanceDto
    {
        public string Address { get; set; }

        public BigInteger Amount { get; set; }
    }
}
