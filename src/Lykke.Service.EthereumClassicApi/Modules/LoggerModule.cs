using Autofac;
using Common.Log;
using Lykke.SlackNotifications;

namespace Lykke.Service.EthereumClassicApi.Modules
{
    public class LoggerModule : Module
    {
        private readonly ILog _log;
        private readonly ISlackNotificationsSender _notificationsSender;

        public LoggerModule(
            ILog log,
            ISlackNotificationsSender notificationsSender)
        {
            _log = log;
            _notificationsSender = notificationsSender;
        }


        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx => _log)
                .As<ILog>()
                .SingleInstance();

            builder
                .Register(ctx => _notificationsSender)
                .As<ISlackNotificationsSender>()
                .SingleInstance();
        }
    }
}
