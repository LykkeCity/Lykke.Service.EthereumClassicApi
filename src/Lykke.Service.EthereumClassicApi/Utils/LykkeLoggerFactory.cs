using System;
using AzureStorage.Tables;
using Common.Log;
using Lykke.AzureQueueIntegration;
using Lykke.AzureQueueIntegration.Publisher;
using Lykke.Logs;
using Lykke.Service.EthereumClassicApi.Common;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.Service.EthereumClassicApi.Logger;
using Lykke.SettingsReader;
using Lykke.SlackNotification.AzureQueue;
using Lykke.SlackNotifications;

namespace Lykke.Service.EthereumClassicApi.Utils
{
    internal class LykkeLoggerFactory
    {
        public static (ILog Log, ISlackNotificationsSender NotificationsSender) CreateLykkeLoggers(
            IReloadingManager<AppSettings> settings)
        {
            var connectionString = settings.Nested(x => x.EthereumClassicApi.Db.LogsConnectionString);
            var slackSettings = settings.CurrentValue?.SlackNotifications?.AzureQueue;
            var consoleLogger = new LogToConsole();


            ILog log;
            ISlackNotificationsSender notificationSender;

            try
            {
                log = new AggregateLogger
                (
                    consoleLogger,
                    CreatePersistenceLogger(connectionString, consoleLogger)
                );
            }
            catch (Exception e)
            {
                log = consoleLogger;

                log.WriteWarningAsync
                (
                    nameof(LykkeLoggerFactory),
                    nameof(CreatePersistenceLogger),
                    "",
                    e.Message,
                    e,
                    DateTime.UtcNow
                );
            }

            try
            {
                notificationSender = CreateNotificationSender(slackSettings, log);
            }
            catch (Exception e)
            {
                notificationSender = new NoNotificationSender();

                log.WriteWarningAsync
                (
                    nameof(LykkeLoggerFactory),
                    nameof(CreateNotificationSender),
                    "",
                    e.Message,
                    e,
                    DateTime.UtcNow
                );
            }


            return (log, notificationSender);
        }

        private static ISlackNotificationsSender CreateNotificationSender(AzureQueuePublicationSettings slackSettings,
            ILog log)
        {
            var connectionString = slackSettings?.ConnectionString;
            var queueName = slackSettings?.QueueName;


            if (string.IsNullOrEmpty(connectionString) || string.IsNullOrEmpty(queueName))
            {
                throw new InvalidOperationException("Slack notifications settings are not specified in settings file.");
            }

            if (IsSettingPlaceholder(connectionString))
            {
                throw new InvalidOperationException(
                    $"Slack notifications connection string [{connectionString}] is not specified in key-value pairs.");
            }

            if (IsSettingPlaceholder(queueName))
            {
                throw new InvalidOperationException(
                    $"Slack notifications queue name [{queueName}] is not specified in key-value pairs.");
            }

            var azureQueuePublisher = new AzureQueuePublisher<SlackMessageQueueEntity>
            (
                Constants.ApplicationName,
                new AzureQueueSettings
                {
                    ConnectionString = connectionString,
                    QueueName = queueName
                }
            );

            azureQueuePublisher
                .SetSerializer(new SlackNotificationsSerializer())
                .SetLogger(log)
                .Start();

            return new SlackNotificationsSender(azureQueuePublisher);
        }

        private static ILog CreatePersistenceLogger(IReloadingManager<string> connectionString, ILog consoleLogger)
        {
            if (string.IsNullOrEmpty(connectionString.CurrentValue))
            {
                throw new InvalidOperationException(
                    "Persistence logger connection string is not specified in settings file.");
            }

            if (IsSettingPlaceholder(connectionString.CurrentValue))
            {
                throw new InvalidOperationException(
                    $"Persistence logger connection string [{connectionString}] is not specified in key-value pairs.");
            }

            var persistenceManager = new LykkeLogToAzureStoragePersistenceManager
            (
                AzureTableStorage<LogEntity>.Create(connectionString, Constants.ApplicationName, consoleLogger),
                consoleLogger
            );

            var persistenceLogger = new LykkeLogToAzureStorage(persistenceManager);

            persistenceLogger.Start();

            return persistenceLogger;
        }

        private static bool IsSettingPlaceholder(string settingValue)
        {
            return settingValue.StartsWith("${") && settingValue.EndsWith("}");
        }
    }
}
