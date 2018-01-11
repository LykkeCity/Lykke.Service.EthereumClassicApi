using Akka.Actor;
using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors
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
