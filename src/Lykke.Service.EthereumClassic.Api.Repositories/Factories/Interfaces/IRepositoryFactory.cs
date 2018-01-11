using Lykke.Service.EthereumClassic.Api.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassic.Api.Repositories.Factories.Interfaces
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
