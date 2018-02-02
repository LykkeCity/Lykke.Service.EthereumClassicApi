using Autofac;
using Lykke.Common.Chaos;
using Lykke.Service.EthereumClassicApi.Common.Settings;
using Lykke.SettingsReader;

namespace Lykke.Service.EthereumClassicApi.Modules
{
    public class SettingsModule : Module
    {
        private readonly IReloadingManager<AppSettings> _appSettings;


        public SettingsModule(
            IReloadingManager<AppSettings> appSettings)
        {
            _appSettings = appSettings;
        }


        protected override void Load(ContainerBuilder builder)
        {
            builder
                .Register(ctx => _appSettings.CurrentValue.EthereumClassicApi)
                .AsSelf()
                .SingleInstance();

            builder
                .Register(ctx => _appSettings.Nested(x => x.EthereumClassicApi.Db))
                .AsSelf()
                .SingleInstance();

            var chaosSettings = _appSettings.CurrentValue.ChaosKitty != null ?
                _appSettings.CurrentValue.ChaosKitty : new ChaosSettings() { StateOfChaos = 0 };


            builder.RegisterChaosKitty(chaosSettings);
        }
    }
}
