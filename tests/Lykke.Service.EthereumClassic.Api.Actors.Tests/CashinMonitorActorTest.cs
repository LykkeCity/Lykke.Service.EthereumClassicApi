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
//    public class CashinMonitorActorTest : BaseActorTest
//    {
//        [ClassInitialize]
//        public static void ClassInit(TestContext context)
//        {

//        }

//        //[TestMethod]
//        //public void CheckPendingCashin__SuccesfulTransaction()
//        //{
//        //    #region Arrange
//        //
//        //    CheckPendingCashin checkPendingCashin = new CheckPendingCashin(1000,
//        //        Guid.NewGuid(), 
//        //        "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A", 
//        //        "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c",
//        //         Guid.NewGuid());
//        //
//        //    Mock<ICashinMonitorRole> cashinMonitorRole = new Mock<ICashinMonitorRole>();
//        //    cashinMonitorRole.Setup(x => x.GetCashinTransactionStateAsync(It.IsAny<string>()))
//        //        .Returns(Task.FromResult(new CashinTransactionState() { Completed = true }));
//        //    
//        //    cashinMonitorRole.Setup(x => x.SendCashinNotificationAsync(checkPendingCashin.Amount,
//        //        checkPendingCashin.OperationId,
//        //        checkPendingCashin.PublicAddress,
//        //        checkPendingCashin.TxHash,
//        //        checkPendingCashin.WalletId)).Returns(Task.FromResult(0)).Verifiable();
//        //
//        //    RegisterMocksForTest((containerBuilder) =>
//        //    {
//        //        containerBuilder.RegisterInstance(cashinMonitorRole.Object).As<ICashinMonitorRole>();
//        //    });
//        //
//        //    #endregion Arrange
//        //
//        //    #region Act
//        //
//        //    TestActorRef<CashinMonitorActor> cashinMonitorActor =
//        //        ActorOfAsTestActorRef<CashinMonitorActor>(_actorSystem.DI().Props<CashinMonitorActor>());
//        //    cashinMonitorActor.Tell(checkPendingCashin);
//        //
//        //    #endregion Act
//        //
//        //    #region Assert
//        //
//        //    ExpectMsg<CashinCompleted>((message) => 
//        //    {
//        //        Assert.AreEqual(checkPendingCashin.Amount, message.Amount);
//        //        Assert.AreEqual(checkPendingCashin.OperationId, message.OperationId);
//        //        Assert.AreEqual(checkPendingCashin.PublicAddress, message.PublicAddress);
//        //        Assert.AreEqual(checkPendingCashin.WalletId, message.WalletId);
//        //        Assert.AreEqual(checkPendingCashin.TxHash, message.TxHash);
//        //    }, TimeSpan.FromSeconds(3));
//        //    
//        //    cashinMonitorRole.Verify(x => x.SendCashinNotificationAsync(checkPendingCashin.Amount,
//        //        checkPendingCashin.OperationId,
//        //        checkPendingCashin.PublicAddress,
//        //        checkPendingCashin.TxHash,
//        //        checkPendingCashin.WalletId));
//        //
//        //    #endregion Asssert
//        //}

//        //[TestMethod]
//        //public void CheckPendingCashin__RescheduleCheck()
//        //{
//        //    #region Arrange
//        //
//        //    CheckPendingCashin checkPendingCashin = new CheckPendingCashin(1000,
//        //        Guid.NewGuid(),
//        //        "0xD143cB0C11fAA3447d0bbe8A49c22853260B406A",
//        //        "0x59ea31b657018f6ae934c1eb46f1cb25ad26914b12ba2aa738a96d8517e7404c",
//        //         Guid.NewGuid());
//        //
//        //    Mock<ICashinMonitorRole> cashinMonitorRole = new Mock<ICashinMonitorRole>();
//        //    cashinMonitorRole.Setup(x => x.GetCashinTransactionStateAsync(It.IsAny<string>()))
//        //        .Returns(Task.FromResult(new CashinTransactionState() { Completed = false }));
//        //    
//        //    cashinMonitorRole.Setup(x => x.SendCashinNotificationAsync(checkPendingCashin.Amount,
//        //        checkPendingCashin.OperationId,
//        //        checkPendingCashin.PublicAddress,
//        //        checkPendingCashin.TxHash,
//        //        checkPendingCashin.WalletId)).Returns(Task.FromResult(0)).Verifiable();
//        //
//        //    RegisterMocksForTest((containerBuilder) =>
//        //    {
//        //        containerBuilder.RegisterInstance(cashinMonitorRole.Object).As<ICashinMonitorRole>();
//        //    });
//        //
//        //    #endregion Arrange
//        //
//        //    #region Act
//        //
//        //    TestActorRef<CashinMonitorActor> cashinMonitorActor =
//        //        ActorOfAsTestActorRef<CashinMonitorActor>(_actorSystem.DI().Props<CashinMonitorActor>());
//        //    cashinMonitorActor.Tell(checkPendingCashin);
//        //    ((Akka.TestKit.TestScheduler)Sys.Scheduler).Advance(TimeSpan.FromSeconds(10));
//        //    Thread.Sleep(500);
//        //
//        //    #endregion Act
//        //
//        //    #region Assert
//        //
//        //    cashinMonitorRole.Verify(x => x.GetCashinTransactionStateAsync(checkPendingCashin.TxHash), Times.Between(2, 3, Range.Inclusive));
//        //
//        //    #endregion Asssert
//        //}
//    }
//}
