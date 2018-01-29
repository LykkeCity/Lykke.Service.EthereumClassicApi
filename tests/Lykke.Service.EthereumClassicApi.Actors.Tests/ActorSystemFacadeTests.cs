//using Lykke.Service.EthereumClassic.Api.Actors.Factories.Interfaces;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;

//namespace Lykke.Service.EthereumClassic.Api.Actors.Tests
//{
//    [TestClass]
//    public class ActorSystemFacadeTests
//    {
//        [TestMethod]
//        public void Ctor__RootActorsCreated()
//        {
//            var actorBuilderMock = new Mock<IRootActorFactory>();

//            new ActorSystemFacade(actorBuilderMock.Object);

//            actorBuilderMock.Verify(c => c.Build<BalanceReadersDispatcherActor>
//            (

//                It.Is<string>(x => x == "balance-readers-dispacther")

//            ), Times.Once);

//            actorBuilderMock.Verify(c => c.Build<CashinProcessorsDispatcherActor>
//            (

//                It.Is<string>(x => x == "cashin-processors-dispacther")

//            ), Times.Once);

//            actorBuilderMock.Verify(c => c.Build<CashoutProcessorsDispatcherActor>
//            (

//                It.Is<string>(x => x == "cashout-processors-dispacther")

//            ), Times.Once);
//        }
//    }
//}


