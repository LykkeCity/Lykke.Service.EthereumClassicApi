using System.Numerics;
using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    public class GasPriceEntity : AzureTableEntity
    {
        public BigInteger Max { get; set; }

        public BigInteger Min { get; set; }
    }
}
