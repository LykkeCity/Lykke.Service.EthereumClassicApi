using Lykke.Common.Chaos;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.EthereumClassicApi.Common.Settings
{
    public class AppSettings
    {
        public EthereumClassicApiSettings EthereumClassicApi { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }

        [Optional]
        public ChaosSettings ChaosKitty { get; set; }
    }
}
