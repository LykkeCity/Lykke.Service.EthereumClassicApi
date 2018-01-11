using Lykke.AzureStorage.Tables;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Entities
{
    public class BalanceEntity : AzureTableEntity
    {
        public string Address { get; set; }

        public string Balance { get; set; }
    }
}
