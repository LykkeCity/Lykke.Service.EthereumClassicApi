using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Entities
{
    public class ObservableBalanceEntity : AzureTableEntity
    {
        public string Address { get; set; }
    }
}
