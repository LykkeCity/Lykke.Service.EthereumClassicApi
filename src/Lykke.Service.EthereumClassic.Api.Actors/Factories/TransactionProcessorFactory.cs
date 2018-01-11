using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Actors.Factories
{
    public class TransactionProcessorsFactory : ChildActorFactory<TransactionProcessorActor>, ITransactionProcessorFactory
    {

    }
}
