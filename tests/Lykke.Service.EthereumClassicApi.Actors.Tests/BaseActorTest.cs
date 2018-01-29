//using Akka.Actor;
//using Akka.TestKit;
//using Akka.TestKit.VsTest;
//using Autofac;
//using Lykke.Service.EthereumClassic.Api.Actors.Extensions;
//using Lykke.Service.EthereumClassic.Api.Common;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Text;
//using Akka.TestKit.Configs;
//using Lykke.Service.EthereumClassic.Api.Common.Settings;

//namespace Lykke.Service.EthereumClassic.Api.Actors.Tests
//{
//    //Inherit if you want to test actors interactions
//    [TestClass]
//    public class BaseActorTest : TestKit
//    {
//        //rewritten on each test
//        protected ActorSystem _actorSystem;
//        protected IContainer _container;
//        private ContainerBuilder _containerBuilder;

//        public BaseActorTest() : base(TestConfigs.TestSchedulerConfig)
//        {

//        }

//        [TestInitialize]
//        public virtual void BaseActorTestInit()
//        {
//            var configuration = new EthereumClassicApiSettings();
//            //INFO: There is no other way to add DI either then this

//            #region You will not google it

//            var type = typeof(TestKitBase);
//            var field = type.GetField("_testState", BindingFlags.NonPublic | BindingFlags.Instance);
//            var state = field.GetValue(this);
//            var testStateType = state.GetType();
//            var testActorSystemProp = testStateType.GetProperty("System", BindingFlags.Public | BindingFlags.Instance);
//            var actorsModule = new ActorsModule(configuration);

//            #endregion

//            _containerBuilder = new ContainerBuilder();

//            _containerBuilder
//                .RegisterAssemblyTypes(typeof(ActorsModule).Assembly)
//                .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(ActorBase)));

//            _actorSystem = (ActorSystem)testActorSystemProp.GetValue(state);
//        }

//        //Call one of the methods below to register dependencies for akk test kit
//        public void RegisterMocksForTest(IEnumerable<(Type interfaceType, object implementation)> interfaceImplementationPairs)
//        {
//            foreach (var (interfaceType, implementation) in interfaceImplementationPairs)
//            {
//                if (implementation.GetType() != interfaceType)
//                    throw new Exception($"{implementation.GetType().FullName} can not be resolved as {interfaceType.FullName}");
//                _containerBuilder.RegisterInstance(implementation).As(interfaceType);
//            }

//            _container = _containerBuilder.Build();
//            _actorSystem.WithContainer(_container);
//        }

//        public void RegisterMocksForTest(Action<ContainerBuilder> registerDependenciesFunc)
//        {
//            registerDependenciesFunc(_containerBuilder);

//            _container = _containerBuilder.Build();
//            _actorSystem.WithContainer(_container);
//        }

//        public TestProbe GetTestProbeWithInfiniteAutoPilot()
//        {
//            TestProbe probe = this.CreateTestProbe();
//            DelegateAutoPilot handler = new DelegateAutoPilot((sender, message) =>
//            {
//                return AutoPilot.KeepRunning;
//            });

//            probe.SetAutoPilot(handler);

//            return probe;
//        }
//    }
//}


