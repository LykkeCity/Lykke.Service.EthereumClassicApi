using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Strategies;
using Lykke.SettingsReader;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private const string BalanceTable              = "Balances";
        private const string DynamicSettingsTable      = "DynamicSettings";
        private const string ObservableBalanceTable    = "ObservableBalances";
        private const string OperationTable            = "Operations";
        private const string OperationStateTable       = "OperationStates";
        private const string OperationTransactionTable = "Operationtransactions";


        private readonly IReloadingManager<string> _connectionString;
        private readonly ILog                      _log;


        public RepositoryFactory(
            ILog log,
            IReloadingManager<DbSettings> settings)
        {
            _log              = log;
            _connectionString = settings.ConnectionString(x => x.DataConnectionString);
        }


        public IBalanceRepository BuildBalanceRepository()
        {
            var table = CreateTable<BalanceEntity>(BalanceTable);

            return new BalanceRepository
            (
                new AddOrReplaceStrategy<BalanceEntity>(table),
                new DeleteStrategy<BalanceEntity>(table),
                new GetAllStrategy<BalanceEntity>(table)
            );
        }

        public IGasPriceRepository BuildGasPriceRepository()
        {
            var table = CreateTable<GasPriceEntity>(DynamicSettingsTable);
            
            return new GasPriceRepository
            (
                new AddOrReplaceStrategy<GasPriceEntity>(table),
                new GetStrategy<GasPriceEntity>(table)
            );
        }

        public IObservableBalanceRepository BuildObservableBalanceRepository()
        {
            var table = CreateTable<ObservableBalanceEntity>(ObservableBalanceTable);

            return new ObservableBalanceRepository
            (
                new AddStrategy<ObservableBalanceEntity>(table),
                new DeleteStrategy<ObservableBalanceEntity>(table),
                new ExistsStrategy<ObservableBalanceEntity>(table),
                new GetAllStrategy<ObservableBalanceEntity>(table)
            );
        }

        public IOperationRepository BuildOperationRepository()
        {
            var table = CreateTable<OperationEntity>(OperationTable);

            return new OperationRepository
            (
                new AddStrategy<OperationEntity>(table),
                new DeleteStrategy<OperationEntity>(table),
                new GetAllStrategy<OperationEntity>(table),
                new GetStrategy<OperationEntity>(table)
            );
        }

        public IOperationStateRepository BuildOperationStateRepository()
        {
            var table = CreateTable<OperationStateEntity>(OperationStateTable);

            return new OperationStateRepository
            (
                new AddOrReplaceStrategy<OperationStateEntity>(table),
                new DeleteStrategy<OperationStateEntity>(table),
                new GetStrategy<OperationStateEntity>(table)
            );
        }

        public IOperationTransactionRepository BuildOperationTransactionRepository()
        {
            var table = CreateTable<OperationTransactionEntity>(OperationTransactionTable);

            return new OperationTransactionRepository
            (
                
            );
        }

        private INoSQLTableStorage<T> CreateTable<T>(string tableName)
            where T : AzureTableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, tableName, _log);
        }
    }
}
