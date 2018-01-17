using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces
{
    public interface IRepositoryFactory
    {
        IBuiltTransactionRepository BuildBuiltTransactionRepository();

        IGasPriceRepository BuildGasPriceRepository();

        IObservableBalanceRepository BuildObservableBalanceRepository();
        
        IBroadcastedTransactionRepository BuildBroadcastedTransactionRepository();

        IBroadcastedTransactionStateRepository BuildBroadcastedTransactionStateRepository();
    }
}
