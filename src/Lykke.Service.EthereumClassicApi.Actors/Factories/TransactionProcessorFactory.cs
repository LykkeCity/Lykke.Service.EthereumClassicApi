using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Actors.Factories
{
    public class TransactionProcessorsFactory : ChildActorFactory<TransactionProcessorActor>, ITransactionProcessorFactory
    {

    }
}
