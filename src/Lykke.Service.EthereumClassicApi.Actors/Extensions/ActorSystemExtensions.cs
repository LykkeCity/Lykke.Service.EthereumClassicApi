using Akka.Actor;
using Akka.DI.AutoFac;
using Autofac;

namespace Lykke.Service.EthereumClassicApi.Actors.Extensions
{
    internal static class ActorSystemExtensions
    {
        public static ActorSystem WithContainer(this ActorSystem system, IContainer container)
        {
            // It's the way, how we add dependency injection ot Akka.net
            // ReSharper disable once UnusedVariable
            var propsResolver = new AutoFacDependencyResolver(container, system);
            
            return system;
        }
    }
}