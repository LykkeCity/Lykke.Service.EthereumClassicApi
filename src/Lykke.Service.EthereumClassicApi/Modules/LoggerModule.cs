using Autofac;
using Common.Log;

namespace Lykke.Service.EthereumClassicApi.Modules
{
    public class LoggerModule : Module
    {
        private readonly ILog _log;

        public LoggerModule(
            ILog log)
        {
            _log = log;
        }


        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx => _log)
                .As<ILog>()
                .SingleInstance();
        }
    }
}
