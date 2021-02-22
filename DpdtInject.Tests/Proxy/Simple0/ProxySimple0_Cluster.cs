using System;
using System.Threading;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Tests.Proxy.Simple0
{
    public partial class ProxySimple0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<SessionSaver>()
                .To<SessionSaver>()
                .WithSingletonScope()
                ;

            Bind<ICalculator>()
                .To<Calculator>()
                .WithSingletonScope()
                .When(rt => rt.WhenInjectedExactlyInto<ProxyCalculator>())
                ;

            Bind<ICalculator>()
                .ToProxy<ProxyCalculator>()
                .WithProxySettings<TelemetryAttribute, SessionSaver>()
                .WithSingletonScope()
                //.When(rt => true)
                .When(rt => rt.WhenInjectedExactlyNotInto<ProxyCalculator>())
                ;

        }

        public class ProxySimple0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ProxySimple0_Cluster>(
                    null
                    );

                var calculator = cluster.Get<ICalculator>();
                Assert.IsNotNull(calculator);

                const int InitialValue = 1;

                var wopIncremented = calculator.DoIncrementWithoutProxy(InitialValue);
                Assert.AreEqual(InitialValue + 1, wopIncremented);
                Assert.IsFalse(SessionSaver.ProxyWasInDeal);

                calculator.DoNothing();
                Assert.IsTrue(Calculator.DoNothingExecuted);

                var one = calculator.GetOne();
                Assert.AreEqual(1, one);

                var wpIncremented = calculator.DoIncrement(InitialValue);
                Assert.AreEqual(InitialValue + 1, wpIncremented);

                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(Calculator).FullName, SessionSaver.FullClassName);
                Assert.AreEqual(nameof(Calculator.DoIncrement), SessionSaver.MemberName);
                Assert.IsTrue(SessionSaver.TakenInSeconds > 0.0);
                Assert.IsNull(SessionSaver.Exception);
                Assert.IsNotNull(SessionSaver.Arguments);
                Assert.AreEqual(2, SessionSaver.Arguments.Length);
                Assert.AreEqual(Calculator.ArgumentName, SessionSaver.Arguments[0] as string);
                Assert.AreEqual(InitialValue, (int)SessionSaver.Arguments[1]);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class TelemetryAttribute : Attribute
    {
    }


    public class SessionSaver : BaseSessionSaver
    {
        public static bool ProxyWasInDeal
        {
            get;
            private set;
        }

        public static string FullClassName
        {
            get;
            private set;
        }

        public static string MemberName
        {
            get;
            private set;
        }

        public static double TakenInSeconds
        {
            get;
            private set;
        }

        public static Exception? Exception
        {
            get;
            private set;
        }

        public static object[]? Arguments
        {
            get;
            private set;
        }

        public override void FixSessionSafely(
            in string fullClassName,
            in string memberName,
            in double takenInSeconds,
            Exception? exception,
            in object[]? arguments
            )
        {
            ProxyWasInDeal = true;
            FullClassName = fullClassName;
            MemberName = memberName;
            TakenInSeconds = takenInSeconds;
            Exception = exception;
            Arguments = arguments;
        }

    }


    public interface ICalculator
    {
        [Telemetry]
        void DoNothing(
            );

        [Telemetry]
        int GetOne(
            );

        [Telemetry]
        int DoIncrement(
            int number
            );

        int DoIncrementWithoutProxy(
            int number
            );

    }

    public class Calculator : ICalculator
    {
        public static string ArgumentName = string.Empty;

        public static bool DoNothingExecuted = false;

        /// <inheritdoc />
        public void DoNothing()
        {
            DoNothingExecuted = true;
        }

        /// <inheritdoc />
        public int GetOne()
        {
            return 1;
        }

        /// <inheritdoc />
        public int DoIncrement(
            int number
            )
        {
            //just for help in case of argument rename
            ArgumentName = nameof(number);
            
            Thread.Sleep(100);

            return number + 1;
        }

        /// <inheritdoc />
        public int DoIncrementWithoutProxy(
            int number
            )
        {
            return number + 1;
        }
    }

    public partial class ProxyCalculator : IFakeProxy<ICalculator>
    {

    }

}
