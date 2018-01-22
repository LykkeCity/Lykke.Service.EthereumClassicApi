using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassicApi.Repositories.Entities
{
    public class ObservableBalanceLockEntity : AzureTableEntity
    {
        public string Address { get; set; }
    }
}
