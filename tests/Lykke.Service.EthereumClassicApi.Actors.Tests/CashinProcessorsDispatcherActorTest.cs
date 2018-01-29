//using Akka.Actor;
//using Akka.DI.Core;
//using Akka.TestKit;
//using Akka.TestKit.VsTest;
//using Autofac;
//using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
//using Lykke.Service.EthereumClassic.Api.Actors.Messages;
//using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
//using Lykke.Service.EthereumClassic.Api.Actors.Roles.Models;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Lykke.Service.EthereumClassic.Api.Actors.Tests
//{
//    [TestClass]
//    public class CashinProcessorsDispatcherActorTest : BaseActorTest
//    {
//        [ClassInitialize]
//        public static void ClassInit(TestContext context)
//        {

//        }

//        [TestMethod]
//        public void Ctor__ResumePendingCashins()
//        {
//            #region Arrange

//            var pendingCashin = new PendingCashin()
//            {
//                Address = "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                Amount = 1000,
//                OperationId = Guid.NewGuid(),
//                TxHash = "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c"
//            };
//            var cashinProcessorFactoryMock = new Mock<ICashinProcessorFactory>();
//            var cashinProcessorsDispatcherRoleMock = new Mock<ICashinProcessorsDispatcherRole>();
//            var cashinMonitorsFactoryMock = new Mock<ICashinMonitorsFactory>();
//            var cashinProcessorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();

//            cashinProcessorFactoryMock.Setup(x => x.Build(It.IsAny<IUntypedActorContext>(), It.IsAny<string>())).Returns(cashinProcessorProbe);
//            cashinMonitorsFactoryMock.Setup(x => x.Build(It.IsAny<IUntypedActorContext>(), It.IsAny<string>())).Returns(cashinMonitorProbe);
//            cashinProcessorsDispatcherRoleMock.Setup(x => x.GetPendingCashinsAsync()).Returns(Task.FromResult((IEnumerable<PendingCashin>)
//                new PendingCashin[]
//                {
//                    pendingCashin
//                }));


//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorFactoryMock.Object).As<ICashinProcessorFactory>();
//                containerBuilder.RegisterInstance(cashinProcessorsDispatcherRoleMock.Object).As<ICashinProcessorsDispatcherRole>();
//                containerBuilder.RegisterInstance(cashinMonitorsFactoryMock.Object).As<ICashinMonitorsFactory>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorsDispatcherActor =
//                ActorOfAsTestActorRef<CashinProcessorsDispatcherActor>(_actorSystem.DI().Props<CashinProcessorsDispatcherActor>());


//            #endregion Act

//            #region Assert

//            cashinProcessorProbe.ExpectMsg<ResumePendingCashin>((message) => 
//            {
//                Assert.AreEqual(pendingCashin.Amount, message.Amount);
//                Assert.AreEqual(pendingCashin.OperationId, message.OperationId);
//                Assert.AreEqual(pendingCashin.Address, message.Address);
//                Assert.AreEqual(pendingCashin.TxHash, message.TxHash);
//                Assert.AreEqual(cashinMonitorProbe, message.CashinMonitors);
//            },TimeSpan.FromSeconds(1));

//            #endregion Asssert
//        }

//        [TestMethod]
//        public void NonZeroBalanceDetected__ProcessMessage()
//        {
//            #region Arrange

//            var nonZeroBalanceDetected = new NonZeroBalanceDetected("0xD143cB0C11fAA3447d0bbe8A49c22853260B406A", 1000);
//            var cashinProcessorFactoryMock = new Mock<ICashinProcessorFactory>();
//            var cashinProcessorsDispatcherRoleMock = new Mock<ICashinProcessorsDispatcherRole>();
//            var cashinMonitorsFactoryMock = new Mock<ICashinMonitorsFactory>();
//            var cashinProcessorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();

//            cashinProcessorFactoryMock.Setup(x => x.Build(It.IsAny<IUntypedActorContext>(), It.IsAny<string>())).Returns(cashinProcessorProbe);
//            cashinMonitorsFactoryMock.Setup(x => x.Build(It.IsAny<IUntypedActorContext>(), It.IsAny<string>())).Returns(cashinMonitorProbe);

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorFactoryMock.Object).As<ICashinProcessorFactory>();
//                containerBuilder.RegisterInstance(cashinProcessorsDispatcherRoleMock.Object).As<ICashinProcessorsDispatcherRole>();
//                containerBuilder.RegisterInstance(cashinMonitorsFactoryMock.Object).As<ICashinMonitorsFactory>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorsDispatcherActor =
//                ActorOfAsTestActorRef<CashinProcessorsDispatcherActor>(_actorSystem.DI().Props<CashinProcessorsDispatcherActor>());

//            cashinProcessorsDispatcherActor.Tell(nonZeroBalanceDetected);

//            #endregion Act

//            #region Assert

//            cashinProcessorProbe.ExpectMsg<PerformCashin>((message) =>
//            {
//                Assert.AreEqual(nonZeroBalanceDetected.Amount, message.Amount);
//                Assert.AreEqual(nonZeroBalanceDetected.Address, message.Address);
//                Assert.AreEqual(cashinMonitorProbe, message.CashinMonitors);
//            }, TimeSpan.FromSeconds(1));

//            #endregion Asssert
//        }
//    }
//}


