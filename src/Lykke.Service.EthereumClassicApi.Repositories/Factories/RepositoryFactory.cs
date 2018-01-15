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
        private const string BalanceTable                     = "Balances";
        private const string BroadcastedTransactionStateTable = "BroadcastedTransactionStates";
        private const string BroadcastedTransactionTable      = "BroadcastedTransactions";
        private const string BuiltTransactionTable            = "BuiltTransactions";
        private const string DynamicSettingsTable             = "DynamicSettings";
        private const string ObservableBalanceTable           = "ObservableBalances";
        
        
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

        public IBroadcastedTransactionStateRepository BuildBroadcastedTransactionStateRepository()
        {
            var table = CreateTable<BroadcastedTransactionStateEntity>(BroadcastedTransactionStateTable);

            return new BroadcastedTransactionStateRepository
            (
                new AddOrReplaceStrategy<BroadcastedTransactionStateEntity>(table),
                new DeleteStrategy<BroadcastedTransactionStateEntity>(table),
                new GetStrategy<BroadcastedTransactionStateEntity>(table)
            );
        }

        public IBroadcastedTransactionRepository BuildBroadcastedTransactionRepository()
        {
            //TODO: Add implementation

            var table = CreateTable<BroadcastedTransactionEntity>(BroadcastedTransactionTable);

            return new BroadcastedTransactionRepository
            (

            );
        }

        public IBuiltTransactionRepository BuildBuiltTransactionRepository()
        {
            var table = CreateTable<BuiltTransactionEntity>(BuiltTransactionTable);

            return new BuiltTransactionRepository
            (
                new AddStrategy<BuiltTransactionEntity>(table),
                new DeleteStrategy<BuiltTransactionEntity>(table),
                new GetAllStrategy<BuiltTransactionEntity>(table),
                new GetStrategy<BuiltTransactionEntity>(table)
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
        
        private INoSQLTableStorage<T> CreateTable<T>(string tableName)
            where T : AzureTableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, tableName, _log);
        }
    }
}
