using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    public class ObservableBalanceEntity : AzureTableEntity
    {
        public string Address { get; set; }

        public string Amount { get; set; }
    }
}
