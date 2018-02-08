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
//using System.Threading;
//using System.Threading.Tasks;

//namespace Lykke.Service.EthereumClassic.Api.Actors.Tests
//{
//    [TestClass]
//    public class CashinProcessorActorTest : BaseActorTest
//    {
//        [ClassInitialize]
//        public static void ClassInit(TestContext context)
//        {

//        }

//        #region Idle

//        [TestMethod]
//        public void CheckPendingCashin__IdleState__SuccesfulTransaction()
//        {
//            #region Arrange

//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var txHash = "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c";
//            var performCashin = new PerformCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid()
//            );

//            var cashinProcessorRole = new Mock<ICashinProcessorRole>();

//            cashinProcessorRole.Setup(x =>
//                    x.BeginCashinOperationAsync(performCashin.Address, performCashin.Amount, performCashin.OperationId))
//            .Returns(Task.FromResult(txHash)).Verifiable();

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorRole.Object).As<ICashinProcessorRole>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorActor =
//                ActorOfAsTestActorRef<CashinProcessorActor>(_actorSystem.DI().Props<CashinProcessorActor>());

//            cashinProcessorActor.Tell(performCashin);

//            #endregion Act

//            #region Assert

//            cashinMonitorProbe.ExpectMsg<CheckPendingCashin>((message) => 
//            {
//                Assert.AreEqual(performCashin.Amount, message.Amount);
//                Assert.AreEqual(performCashin.CashinMonitors, cashinMonitorProbe);
//                Assert.AreEqual(performCashin.OperationId, message.OperationId);
//                Assert.AreEqual(txHash, message.TxHash);
//            }, TimeSpan.FromSeconds(1));

//            #endregion Asssert
//        }

//        [TestMethod]
//        public void ResumePendingCashin__IdleState__SuccesfulTransaction()
//        {
//            #region Arrange

//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var txHash = "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c";
//            var resumePendingCashin = new ResumePendingCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid(),
//                txHash
//            );

//            var cashinProcessorRole = new Mock<ICashinProcessorRole>();

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorRole.Object).As<ICashinProcessorRole>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorActor =
//                ActorOfAsTestActorRef<CashinProcessorActor>(_actorSystem.DI().Props<CashinProcessorActor>());

//            cashinProcessorActor.Tell(resumePendingCashin);

//            #endregion Act

//            #region Assert

//            cashinMonitorProbe.ExpectMsg<CheckPendingCashin>((message) =>
//            {
//                Assert.AreEqual(resumePendingCashin.Amount, message.Amount);
//                Assert.AreEqual(resumePendingCashin.CashinMonitors, cashinMonitorProbe);
//                Assert.AreEqual(resumePendingCashin.OperationId, message.OperationId);
//                Assert.AreEqual(txHash, message.TxHash);
//            }, TimeSpan.FromSeconds(1));

//            #endregion Asssert
//        }

//        #endregion Idle

//        #region Busy

//        [TestMethod]
//        public void CheckPendingCashin__BusyState__SuccesfulTransaction()
//        {
//            #region Arrange

//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var txHash = "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c";
//            var performCashin = new PerformCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid()
//            );

//            var cashinProcessorRole = new Mock<ICashinProcessorRole>();
//            cashinProcessorRole.Setup(x =>
//                x.BeginCashinOperationAsync(performCashin.Address, performCashin.Amount, performCashin.OperationId))
//            .Returns(Task.FromResult(txHash)).Verifiable();

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorRole.Object).As<ICashinProcessorRole>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorActor =
//                ActorOfAsTestActorRef<CashinProcessorActor>(_actorSystem.DI().Props<CashinProcessorActor>());

//            cashinProcessorActor.Tell(performCashin);
//            cashinProcessorActor.Tell(performCashin);

//            #endregion Act

//            #region Assert

//            cashinMonitorProbe.ExpectMsg<CheckPendingCashin>();
//            cashinMonitorProbe.ExpectNoMsg();

//            #endregion Asssert
//        }

//        [TestMethod]
//        public void ResumePendingCashin__BusyState__SuccesfulTransaction()
//        {
//            #region Arrange

//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var txHash = "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c";
//            var resumePendingCashin = new ResumePendingCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid(),
//                txHash
//            );

//            var performCashin = new PerformCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid()
//            );

//            var cashinProcessorRole = new Mock<ICashinProcessorRole>();
//            cashinProcessorRole.Setup(x =>
//                x.BeginCashinOperationAsync(performCashin.Address, performCashin.Amount, performCashin.OperationId))
//            .Returns(Task.FromResult(txHash)).Verifiable();

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorRole.Object).As<ICashinProcessorRole>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorActor =
//                ActorOfAsTestActorRef<CashinProcessorActor>(_actorSystem.DI().Props<CashinProcessorActor>());

//            cashinProcessorActor.Tell(performCashin);
//            cashinProcessorActor.Tell(resumePendingCashin);

//            #endregion Act

//            #region Assert

//            cashinMonitorProbe.ExpectMsg<CheckPendingCashin>();
//            cashinMonitorProbe.ExpectNoMsg();
//            var stashedMessages = cashinProcessorActor.UnderlyingActor.Stash.ClearStash();
//            Assert.IsNotNull(stashedMessages);
//            var resumePendingCashinActual = (ResumePendingCashin)stashedMessages.FirstOrDefault().Message;
//            Assert.AreEqual(resumePendingCashin.OperationId, resumePendingCashinActual.OperationId);
//            Assert.AreEqual(resumePendingCashin.Address, resumePendingCashinActual.Address);
//            Assert.AreEqual(resumePendingCashin.TxHash, resumePendingCashinActual.TxHash);
//            Assert.AreEqual(resumePendingCashin.Amount, resumePendingCashinActual.Amount);
//            Assert.AreEqual(resumePendingCashin.CashinMonitors, resumePendingCashinActual.CashinMonitors);

//            #endregion Asssert
//        }

//        [TestMethod]
//        public void CashinCompleted__BusyState__SuccesfulTransaction()
//        {
//            #region Arrange

//            var cashinMonitorProbe = GetTestProbeWithInfiniteAutoPilot();
//            var txHash = "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c";
//            var resumePendingCashin = new ResumePendingCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid(),
//                txHash
//            );

//            var performCashin = new PerformCashin
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000,
//                cashinMonitorProbe,
//                Guid.NewGuid()
//            );

//            var cashinCompleted = new CashinCompleted
//            (
//                "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//                1000, 
//                false,
//                Guid.NewGuid(),
//                txHash
//            );

//            var cashinProcessorRole = new Mock<ICashinProcessorRole>();
//            cashinProcessorRole.Setup(x =>
//                x.CompleteCashinOperationAsync(cashinCompleted.Address, cashinCompleted.Amount, cashinCompleted.OperationId, txHash))
//            .Returns(Task.FromResult(0)).Verifiable();

//            RegisterMocksForTest((containerBuilder) =>
//            {
//                containerBuilder.RegisterInstance(cashinProcessorRole.Object).As<ICashinProcessorRole>();
//            });

//            #endregion Arrange

//            #region Act

//            var cashinProcessorActor =
//                ActorOfAsTestActorRef<CashinProcessorActor>(_actorSystem.DI().Props<CashinProcessorActor>());

//            cashinProcessorActor.Tell(performCashin);
//            cashinProcessorActor.Tell(resumePendingCashin);
//            cashinProcessorActor.Tell(cashinCompleted);

//            #endregion Act

//            #region Assert

//            cashinProcessorRole.Verify(x => 
//                x.CompleteCashinOperationAsync(cashinCompleted.Address, cashinCompleted.Amount, cashinCompleted.OperationId, txHash), Times.Once);

//            var stashedMessages = cashinProcessorActor.UnderlyingActor.Stash.ClearStash();

//            Assert.IsTrue(!stashedMessages.Any());

//            #endregion Asssert
//        }

//        #endregion Busy
//    }
//}


