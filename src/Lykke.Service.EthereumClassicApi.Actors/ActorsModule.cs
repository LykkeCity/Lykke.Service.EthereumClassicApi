using Akka.Actor;
using Autofac;
using Lykke.Service.EthereumClassicApi.Actors.Factories.Interfaces;
using Lykke.Service.EthereumClassicApi.Actors.Roles.Interfaces;


namespace Lykke.Service.EthereumClassicApi.Actors
{
    public sealed class ActorsModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
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
    }
}
