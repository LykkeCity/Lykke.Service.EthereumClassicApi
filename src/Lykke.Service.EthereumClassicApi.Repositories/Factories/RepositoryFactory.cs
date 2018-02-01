using System.Numerics;
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
using Lykke.Service.EthereumClassicApi.Repositories.Serializers;
using Lykke.SettingsReader;

namespace Lykke.Service.EthereumClassicApi.Repositories.Factories
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private const string DynamicSettingsTable = "DynamicSettings";
        private const string ObservableBalanceTable = "ObservableBalances";
        private const string TransactionTable = "Transactions";


        private readonly IReloadingManager<string> _connectionString;
        private readonly ILog _log;


        public RepositoryFactory(
            ILog log,
            IReloadingManager<DbSettings> settings)
        {
            _log = log;
            _connectionString = settings.ConnectionString(x => x.DataConnectionString);
            
            var provider = new CompositeMetamodelProvider()
                .AddProvider
                (
                    new AnnotationsBasedMetamodelProvider()
                )
                .AddProvider
                (
                    new ConventionBasedMetamodelProvider()
                        .AddTypeSerializerRule
                        (
                            t => t == typeof(BigInteger),
                            s => new BigIntegerSerializer()
                        )
                );

            EntityMetamodel.Configure(provider);
        }

        private INoSQLTableStorage<T> CreateTable<T>(string tableName)
            where T : AzureTableEntity, new()
        {
            return AzureTableStorage<T>.Create(_connectionString, tableName, _log);
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

        public ITransactionRepository BuildTransactionRepository()
        {
            var table = CreateTable<TransactionEntity>(TransactionTable);

            return new TransactionRepository(table);
        }
    }
}
