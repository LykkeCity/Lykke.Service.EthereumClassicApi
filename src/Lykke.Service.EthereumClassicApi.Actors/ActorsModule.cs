using System;
using Akka.Actor;
using Autofac;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureQueueIntegration;
using Lykke.AzureQueueIntegration.Publisher;
using Lykke.Logs;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;
using Lykke.Service.EthereumClassicApi.Blockchain;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Logger;
using Lykke.Service.EthereumClassicApi.Repositories;
using Lykke.Service.EthereumClassicApi.Services;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Lykke.SlackNotifications;


namespace Lykke.Service.EthereumClassicApi.Actors
{
    public sealed class ActorsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _settings;

        public ActorsModule(
            IReloadingManager<AppSettings> settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            (var log, var notificationSender) = CreateLykkeLoggers();

            builder
                .Register(ctx => log)
                .As<ILog>()
                .SingleInstance();

            builder
                .Register(ctx => notificationSender)
                .As<ISlackNotificationsSender>()
                .SingleInstance();

            builder
                .Register(ctx => _settings)
                .As<IReloadingManager<AppSettings>>()
                .SingleInstance();

            builder
                .Register(ctx => _settings.CurrentValue.EthereumClassicApi)
                .As<EthereumClassicApiSettings>()
                .SingleInstance();

            builder
                .RegisterModule<BlockchainModule>()
                .RegisterModule<RepositoriesModule>()
                .RegisterModule<ServicesModule>();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IActorRole>()
                .AsImplementedInterfaces()
                .SingleInstance();
            
            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .AssignableTo<IChildActorFactory>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterAssemblyTypes(ThisAssembly)
                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ActorBase)));
        }

        private (ILog Log, ISlackNotificationsSender NotificationSender) CreateLykkeLoggers()
        {
            var consoleLogger = new LogToConsole();


            ILog                      log                = consoleLogger;
            ISlackNotificationsSender notificationSender = null;


            try
            {
                log = new AggregateLogger
                (
                    consoleLogger,
                    CreatePersistenceLogger(consoleLogger)
                );
            }
            catch (Exception e)
            {
                log = consoleLogger;

                log.WriteWarningAsync
                (
                    component: nameof(ActorsModule),
                    process:   nameof(CreatePersistenceLogger),
                    context:   "",
                    info:      e.Message,
                    ex:        e,
                    dateTime:  DateTime.UtcNow
                );
            }

            try
            {
                notificationSender = CreateNotificationSender(log);
            }
            catch (Exception e)
            {
                notificationSender = new NoNotificationSender();

                log.WriteWarningAsync
                (
                    component: nameof(ActorsModule),
                    process:   nameof(CreateNotificationSender),
                    context:   "",
                    info:      e.Message,
                    ex:        e,
                    dateTime:  DateTime.UtcNow
                );
            }


            return (log, notificationSender);
        }

        private ILog CreatePersistenceLogger(ILog consoleLogger)
        {
            var connectionStringManager = _settings.Nested(x => x.EthereumClassicApi.Db.LogsConnectionString);
            var connectionString        = connectionStringManager.CurrentValue;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Persistence logger connection string is not specified in settings file.");
            }

            if (IsSettingPlaceholder(connectionString))
            {
                throw new InvalidOperationException($"Persistence logger connection string [{connectionString}] is not specified in key-value pairs.");
            }

            var persistenceManager = new LykkeLogToAzureStoragePersistenceManager
            (
                AzureTableStorage<LogEntity>.Create(connectionStringManager, "EthereumClassicApi", consoleLogger),
                consoleLogger
            );

            var persistenceLogger = new LykkeLogToAzureStorage(persistenceManager);

            persistenceLogger.Start();

            return persistenceLogger;
        }

        private ISlackNotificationsSender CreateNotificationSender(ILog log)
        {
            var slackSettings    = _settings.CurrentValue?.SlackNotifications?.AzureQueue;
            var connectionString = slackSettings?.ConnectionString;
            var queueName        = slackSettings?.QueueName;


            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(queueName))
            {
                throw new InvalidOperationException("Slack notifications settings are not specified in settings file.");
            }

            if (IsSettingPlaceholder(connectionString))
            {
                throw new InvalidOperationException($"Slack notifications connection string [{connectionString}] is not specified in key-value pairs.");
            }

            if (IsSettingPlaceholder(queueName))
            {
                throw new InvalidOperationException($"Slack notifications queue name [{queueName}] is not specified in key-value pairs.");
            }

            var azureQueuePublisher = new AzureQueuePublisher<SlackMessageQueueEntity>
            (
                Constants.ApplicationName,
                new AzureQueueSettings
                {
                    ConnectionString = connectionString,
                    QueueName        = queueName
                }
            );

            azureQueuePublisher
                .SetSerializer(new SlackNotificationsSerializer())
                .SetLogger(log)
                .Start();

            return new SlackNotificationsSender(azureQueuePublisher);
        }

        private static bool IsSettingPlaceholder(string settingValue)
        {
            return settingValue.StartsWith("${") && settingValue.EndsWith("}");
        }
    }
}
