using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;

namespace Lykke.Service.EthereumClassicApi.Repositories.Extensions
{
    public static class TransactionEntityExtensions
    {
        public static bool IsFinished(this TransactionEntity entity)
        {
            return entity.State == TransactionState.Completed ||
                   entity.State == TransactionState.Failed;
        }
    }
}
