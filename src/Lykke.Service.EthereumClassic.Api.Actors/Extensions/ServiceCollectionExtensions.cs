using Lykke.Service.EthereumClassic.Api.Common.Settings;
using Lykke.SettingsReader;
using Microsoft.Extensions.DependencyInjection;

namespace Lykke.Service.EthereumClassic.Api.Actors.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddActorSystem(this IServiceCollection services, IReloadingManager<AppSettings> settings)
        {
            var actorSystemFacade = ActorSystemFacadeFactory.Build(settings);

            return services
                .AddSingleton(x => actorSystemFacade);
        }
    }
}