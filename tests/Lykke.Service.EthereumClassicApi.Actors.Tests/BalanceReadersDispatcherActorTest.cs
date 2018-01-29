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
//    public class BalanceReadersDispatcherActorTest : BaseActorTest
//    {
//        [ClassInitialize]
//        public static void ClassInit(TestContext context)
//        {

//        }

//        [TestMethod]
//        public void CheckBalances__BalanceCheckInitiatedForRootActors()
//        {
//            #region Arrange

//            var balanceHolderPublicAddress = "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A";


//            var mockedBalanceReadersDispatcherRole = new Mock<IBalanceReadersDispatcherRole>();
//            var mockedBalanceReadersFactory = new Mock<IBalanceReadersFactory>();
//            var balanceReaderProbe = this.CreateTestProbe();
//            var balanceReaderProbeHandler = new DelegateAutoPilot((sender, message) => AutoPilot.KeepRunning);

//            balanceReaderProbe.SetAutoPilot(balanceReaderProbeHandler);

//            mockedBalanceReadersDispatcherRole.Setup(x => x.GetBalanceHoldersAsync())
//                .Returns(Task.FromResult<IEnumerable<string>>(new []
//                {
//                    balanceHolderPublicAddress
//                }));

//            mockedBalanceReadersFactory.Setup(x => x.Build(It.IsAny<IUntypedActorContext>(), It.IsAny<string>()))
//                .Returns(balanceReaderProbe);

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(mockedBalanceReadersDispatcherRole.Object).As<IBalanceReadersDispatcherRole>();
//                containerBuilder.RegisterInstance(mockedBalanceReadersFactory.Object).As<IBalanceReadersFactory>();
//            });

//            var balanceListenerProbe = this.CreateTestProbe();
//            var handler = new DelegateAutoPilot((sender, message) => AutoPilot.KeepRunning);

//            balanceListenerProbe.SetAutoPilot(handler);

//            #endregion Arrange

//            #region Act

//            var balanceReadersDispatcherActor =
//                ActorOfAsTestActorRef<BalanceReadersDispatcherActor>(_actorSystem.DI().Props<BalanceReadersDispatcherActor>());

//            balanceReadersDispatcherActor.Tell(new CheckBalances(balanceListenerProbe));

//            #endregion Act

//            #region Assert

//            balanceReaderProbe.ExpectMsg<CheckBalance>((message) =>
//            {
//                Assert.IsNotNull(message);
//                Assert.AreEqual(message.BalancesListener, balanceListenerProbe);
//                Assert.AreEqual(balanceHolderPublicAddress, message.Address);
//            }, TimeSpan.FromSeconds(10));

//            #endregion Asssert
//        }
//    }
//}


