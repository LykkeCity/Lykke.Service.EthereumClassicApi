//using System;
//using System.Numerics;
//using System.Threading.Tasks;
//using Akka.DI.Core;
//using Autofac;
//using Lykke.Service.EthereumClassic.Api.Actors.Messages;
//using Lykke.Service.EthereumClassic.Api.Actors.Roles.Interfaces;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace Lykke.Service.EthereumClassic.Api.Actors.Tests
//{
//    [TestClass]
//    public class BalanceReaderActorTest : BaseActorTest
//    {
//        [ClassInitialize]
//        public static void ClassInit(TestContext context)
//        {

//        }

//        [TestMethod]
//        public void CheckBalance__BalanceListenerNotified()
//        {
//            #region Arrange

//            var publicAddress = "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A";
//            var amount        = new BigInteger(1000);

//            var balanceReaderRoleMock = new Mock<IBalanceReaderRole>();
//            balanceReaderRoleMock.Setup(x => x.GetBalanceAsync(publicAddress)).Returns(Task.FromResult(amount));


//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(balanceReaderRoleMock.Object).As<IBalanceReaderRole>();
//            });

//            var balanceListenerProbe = GetTestProbeWithInfiniteAutoPilot();

//            #endregion Arrange

//            #region Act

//            var balanceReaderActor =
//                ActorOfAsTestActorRef<BalanceReaderActor>(_actorSystem.DI().Props<BalanceReaderActor>());

//            balanceReaderActor.Tell(new CheckBalance(publicAddress, balanceListenerProbe));

//            #endregion Act

//            #region Assert

//            balanceListenerProbe.ExpectMsg<NonZeroBalanceDetected>((message) => 
//            {
//                Assert.AreEqual(publicAddress,message.Address);
//                Assert.AreEqual(amount, message.Amount);
//            }, TimeSpan.FromSeconds(10));

//            #endregion Asssert
//        }

//        [TestMethod]
//        public void CheckBalance__BalanceListenerNotificationSkipped()
//        {
//            #region Arrange
//            var publicAddress = "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A";
//            var amount        = new BigInteger(0);

//            var balanceReaderRoleMock = new Mock<IBalanceReaderRole>();
//            balanceReaderRoleMock.Setup(x => x.GetBalanceAsync(publicAddress)).Returns(Task.FromResult(amount));


//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(balanceReaderRoleMock.Object).As<IBalanceReaderRole>();
//            });

//            var balanceListenerProbe = GetTestProbeWithInfiniteAutoPilot();

//            #endregion Arrange

//            #region Act

//            var balanceReaderActor =
//                ActorOfAsTestActorRef<BalanceReaderActor>(_actorSystem.DI().Props<BalanceReaderActor>());

//            balanceReaderActor.Tell(new CheckBalance(publicAddress, balanceListenerProbe));

//            #endregion Act

//            #region Assert

//            balanceListenerProbe.ExpectNoMsg(TimeSpan.FromSeconds(2));

//            #endregion Asssert
//        }
//    }
//}
