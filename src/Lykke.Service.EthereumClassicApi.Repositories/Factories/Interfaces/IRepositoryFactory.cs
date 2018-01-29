using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces
{
    public interface IRepositoryFactory
    {
        IBroadcastedTransactionRepository BuildBroadcastedTransactionRepository();

        IBroadcastedTransactionStateRepository BuildBroadcastedTransactionStateRepository();
        IBuiltTransactionRepository BuildBuiltTransactionRepository();

        IGasPriceRepository BuildGasPriceRepository();

        IObservableBalanceLockRepository BuildObservableBalanceLockRepository();

        IObservableBalanceRepository BuildObservableBalanceRepository();
    }
}
