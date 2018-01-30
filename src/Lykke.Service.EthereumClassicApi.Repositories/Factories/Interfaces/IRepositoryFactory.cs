using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces
{
    public interface IRepositoryFactory
    {
        IGasPriceRepository BuildGasPriceRepository();
        
        IObservableBalanceRepository BuildObservableBalanceRepository();

        ITransactionRepository BuildTransactionRepository();
    }
}
