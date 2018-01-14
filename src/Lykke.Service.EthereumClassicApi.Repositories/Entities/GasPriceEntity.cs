using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    public class GasPriceEntity : AzureTableEntity
    {
        public string Max { get; set; }

        public string Min { get; set; }
    }
}
