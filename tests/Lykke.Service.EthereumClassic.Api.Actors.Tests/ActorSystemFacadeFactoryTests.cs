//using System;
//using Akka.Actor;
//using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
//using Lykke.Service.EthereumClassic.Api.Actors.Messages;
//using Lykke.Service.EthereumClassic.Api.Common;
//using Lykke.Service.EthereumClassic.Api.Common.Settings;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace Lykke.Service.EthereumClassic.Api.Actors.Tests
//{
//    [TestClass]
//    public class ActorSystemFacadeFactoryTests
//    {
//        [TestMethod]
//        public void Build__BalanceChecksScheduled()
//        {
//            var actorBuilderMock               = new Mock<IRootActorFactory>();
//            var balanceReadersDispatcherMock   = new Mock<IActorRef>();
//            var cashinProcessorsDispatcherMock = new Mock<IActorRef>();
//            var configuration                  = new EthereumClassicApiSettings {BalancesCheckInterval = TimeSpan.FromMinutes(10)};
//            var schedulerMock                  = new Mock<IScheduler>();

//            actorBuilderMock
//                .Setup(x => x.Build<BalanceObserverDispatcherActor>(It.IsAny<string>()))
//                .Returns(balanceReadersDispatcherMock.Object);

//            actorBuilderMock
//                .Setup(x => x.Build<CashinProcessorsDispatcherActor>(It.IsAny<string>()))
//                .Returns(cashinProcessorsDispatcherMock.Object);

//            var facadeFactory = new ActorSystemFacadeFactory
//            (
//                actorBuilderMock.Object,
//                configuration,
//                schedulerMock.Object
//            );

//            facadeFactory.Build();

//            schedulerMock.Verify(c => c.ScheduleTellRepeatedly
//            (

//                // ReSharper disable PossibleUnintendedReferenceComparison
//                It.Is<TimeSpan>(x => x == TimeSpan.Zero),
//                It.Is<TimeSpan>(x => x == configuration.BalancesCheckInterval),
//                It.Is<IActorRef>(x => x == balanceReadersDispatcherMock.Object),
//                It.Is<CheckBalances>(x => x.BalancesListener == cashinProcessorsDispatcherMock.Object),
//                It.Is<IActorRef>(x => x == Nobody.Instance)
//                // ReSharper restore PossibleUnintendedReferenceComparison

//            ), Times.Once);
//        }
//    }
//}
