using System;
using System.Threading;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.RContext;
using DpdtInject.Injector.Bind.Settings;

namespace DpdtInject.Tests.Proxy.SimpleWithDefault0
{
    public partial class ProxySimpleWithDefault0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<SessionSaver>()
                .To<SessionSaver>()
                .WithSingletonScope()
                ;

            Bind<IMethodContainer>()
                .To<MethodContainer>()
                .WithSingletonScope()
                .When(rt => rt.WhenInjectedExactlyInto<MethodContainerProxy>())
                ;

            Bind<IMethodContainer>()
                .ToProxy<MethodContainerProxy>()
                .WithProxySettings<TelemetryAttribute, SessionSaver>()
                .WithSingletonScope()
                .Setup<SuppressCircularCheck>()
                .When(rt => rt.WhenInjectedExactlyNotInto<MethodContainerProxy>())
                ;

        }

        public class ProxySimpleWithDefault0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ProxySimpleWithDefault0_Cluster>(
                    null
                    );

                var calculator = cluster.Get<IMethodContainer>();
                Assert.IsNotNull(calculator);

                calculator.DoNothing2();

                calculator.DoNothing();
                Assert.IsTrue(MethodContainer.DoNothingExecuted);

                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(MethodContainer).FullName, SessionSaver.FullClassName);
                Assert.AreEqual(nameof(MethodContainer.DoNothing), SessionSaver.MemberName);
                Assert.IsTrue(SessionSaver.TakenInSeconds > 0.0);
                Assert.IsNull(SessionSaver.Exception);
                Assert.IsNotNull(SessionSaver.Arguments);
                Assert.AreEqual(2, SessionSaver.Arguments.Length);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Method)]
    public class TelemetryAttribute : Attribute
    {
    }


    /// <summary>
    /// In real applications this class MUST be THREAD SAFE!
    /// </summary>
    public class SessionSaver : BaseSessionSaver
    {
        public static Guid SessionGuid
        {
            get;
            private set;
        }

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

        /// <summary>
        /// Fixes the current session to a permanent storage.
        /// DO NOT RAISE AN EXCEPTION HERE!
        /// </summary>
        public override Guid StartSessionSafely(
            in string fullClassName,
            in string memberName,
            in object[]? arguments
            )
        {
            ProxyWasInDeal = true;
            FullClassName = fullClassName;
            MemberName = memberName;
            Arguments = arguments;

            SessionGuid = Guid.NewGuid();
            return SessionGuid;
        }


        public override void FixSessionSafely(
            in Guid sessionGuid,
            in double takenInSeconds,
            Exception? exception
            )
        {
            if (SessionGuid != sessionGuid)
            {
                throw new InvalidOperationException("Session guids are different!");
            }

            TakenInSeconds = takenInSeconds;
            Exception = exception;
        }

    }


    public interface IMethodContainer
    {
        [Telemetry]
        void DoNothing(
            MyReadonlyStruct arg = default
            );

        void DoNothing2(
            MyReadonlyStruct arg = default
            );

    }

    public class MethodContainer : IMethodContainer
    {

        public static bool DoNothingExecuted = false;

        /// <inheritdoc />
        public void DoNothing(
            MyReadonlyStruct arg = default
            )
        {
            DoNothingExecuted = true;
        }

        /// <inheritdoc />
        public void DoNothing2(
            MyReadonlyStruct arg = default
            )
        {
            //nothing to do
        }
    }

    public readonly struct MyReadonlyStruct
    {
    }

    public partial class MethodContainerProxy : IFakeProxy<IMethodContainer>
    {
    }

}
