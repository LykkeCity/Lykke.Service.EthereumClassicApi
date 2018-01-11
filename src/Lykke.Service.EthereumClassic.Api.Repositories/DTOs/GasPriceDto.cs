using System.Numerics;

namespace Lykke.Service.EthereumClassic.Api.Repositories.DTOs
{
    public class GasPriceDto
    {
        public BigInteger Max { get; set; }

        public BigInteger Min { get; set; }
    }
}