using Akka.Actor;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors
{
    public class TransactionStateManagerActor : ReceiveActor
    {
        private readonly ITransactionStateManagerRole _transactionStateManagerRole;

        public TransactionStateManagerActor(
            ITransactionStateManagerRole transactionStateManagerRole)
        {
            _transactionStateManagerRole = transactionStateManagerRole;
        }
    }
}
