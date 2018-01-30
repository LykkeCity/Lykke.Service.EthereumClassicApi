using AzureStorage;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureStorage.Tables;
using Lykke.AzureStorage.Tables.Entity.Metamodel;
using Lykke.AzureStorage.Tables.Entity.Metamodel.Providers;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Repositories.Entities;
using Lykke.Service.EthereumClassicApi.Repositories.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Repositories.Interfaces;
using Lykke.SettingsReader;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private const string BroadcastedTransactionStateTable = "BroadcastedTransactionStates";
        private const string BroadcastedTransactionTable = "BroadcastedTransactions";
        private const string BuiltTransactionTable = "BuiltTransactions";
        private const string DynamicSettingsTable = "DynamicSettings";
        private const string ObservableBalanceTable = "ObservableBalances";


        private readonly IReloadingManager<string> _connectionString;
        private readonly ILog _log;


        public RepositoryFactory(
            ILog log,
            IReloadingManager<DbSettings> settings)
        {
            _log = log;
            _connectionString = settings.ConnectionString(x => x.DataConnectionString);
            

            EntityMetamodel.Configure(new AnnotationsBasedMetamodelProvider());
        }

        private INoSQLTableStorage<T> CreateTable<T>(string tableName)
            where T : AzureTableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, tableName, _log);
        }


        public IBroadcastedTransactionStateRepository BuildBroadcastedTransactionStateRepository()
        {
            var table = CreateTable<BroadcastedTransactionStateEntity>(BroadcastedTransactionStateTable);

            return new BroadcastedTransactionStateRepository(table);
        }

        public IBroadcastedTransactionRepository BuildBroadcastedTransactionRepository()
        {
            var table = CreateTable<BroadcastedTransactionEntity>(BroadcastedTransactionTable);

            return new BroadcastedTransactionRepository(table);
        }

        public IBuiltTransactionRepository BuildBuiltTransactionRepository()
        {
            var table = CreateTable<BuiltTransactionEntity>(BuiltTransactionTable);

            return new BuiltTransactionRepository(table);
        }

        public IGasPriceRepository BuildGasPriceRepository()
        {
            var table = CreateTable<GasPriceEntity>(DynamicSettingsTable);

            return new GasPriceRepository(table);
        }

        public IObservableBalanceRepository BuildObservableBalanceRepository()
        {
            var table = CreateTable<ObservableBalanceEntity>(ObservableBalanceTable);

            return new ObservableBalanceRepository(table);
        }
    }
}
