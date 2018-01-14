using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces
{
    public interface IRepositoryFactory
    {
        IBalanceRepository BuildBalanceRepository();

        IGasPriceRepository BuildGasPriceRepository();

        IObservableBalanceRepository BuildObservableBalanceRepository();

        IOperationRepository BuildOperationRepository();

        IOperationStateRepository BuildOperationStateRepository();

        IOperationTransactionRepository BuildOperationTransactionRepository();
    }
}
