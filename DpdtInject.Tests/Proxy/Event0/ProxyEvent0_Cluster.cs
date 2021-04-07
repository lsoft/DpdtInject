using System;
using System.Threading;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.RContext;
using DpdtInject.Injector.Bind.Settings;
using System.Text;
using System.Linq;

namespace DpdtInject.Tests.Proxy.Event0
{
    public partial class ProxyEvent0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<SessionSaver>()
                .To<SessionSaver>()
                .WithSingletonScope()
                ;

            Bind<IEventContainer>()
                .To<EventContainer>()
                .WithSingletonScope()
                .When(rt => rt.WhenInjectedExactlyInto<EventContainerProxy>())
                ;

            Bind<IEventContainer>()
                .ToProxy<EventContainerProxy>()
                .WithProxySettings<TelemetryAttribute, SessionSaver>()
                .WithSingletonScope()
                .Setup<SuppressCircularCheck>()
                .When(rt => rt.WhenInjectedExactlyNotInto<EventContainerProxy>())
                ;

        }

        public class ProxyEvent0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ProxyEvent0_Cluster>(
                    null
                    );

                var propertyContainer = cluster.Get<IEventContainer>();
                Assert.IsNotNull(propertyContainer);

                propertyContainer.AEvent += a => { };
                Assert.IsFalse(SessionSaver.ProxyWasInDeal);

                SessionSaver.ProxyWasInDeal = false;
                propertyContainer.BEvent += b => 1;
                Assert.IsFalse(SessionSaver.ProxyWasInDeal);

                CDelegate cDelegate = c => null;

                SessionSaver.ProxyWasInDeal = false;
                propertyContainer.CEvent += cDelegate;
                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(EventContainer).FullName, SessionSaver.FullClassName);
                Assert.AreEqual(nameof(EventContainer.CEvent), SessionSaver.MemberName);
                Assert.IsTrue(SessionSaver.TakenInSeconds > 0.0);
                Assert.IsNull(SessionSaver.Exception);
                Assert.IsNotNull(SessionSaver.Arguments);
                Assert.AreEqual(2, SessionSaver.Arguments.Length);
                Assert.IsTrue(
                    Enumerable.SequenceEqual(
                        SessionSaver.Arguments,
                        new object[] { "value", cDelegate }
                        )
                    );

                SessionSaver.ProxyWasInDeal = false;
                propertyContainer.CEvent -= cDelegate;
                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(EventContainer).FullName, SessionSaver.FullClassName);
                Assert.AreEqual(nameof(EventContainer.CEvent), SessionSaver.MemberName);
                Assert.IsTrue(SessionSaver.TakenInSeconds > 0.0);
                Assert.IsNull(SessionSaver.Exception);
                Assert.IsNotNull(SessionSaver.Arguments);
                Assert.AreEqual(2, SessionSaver.Arguments.Length);
                Assert.IsTrue(
                    Enumerable.SequenceEqual(
                        SessionSaver.Arguments,
                        new object[] { "value", cDelegate }
                        )
                    );

            }
        }
    }


    [AttributeUsage(AttributeTargets.Event | AttributeTargets.Property | AttributeTargets.Method)]
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
            set;
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

    public delegate void ADelegate(int a);
    public delegate ulong BDelegate(ulong b);
    public delegate Encoding CDelegate(StringBuilder c);

    public interface IEventContainer
    {
        event ADelegate AEvent;

        event BDelegate BEvent;

        [Telemetry]
        event CDelegate CEvent;
    }

    public class EventContainer : IEventContainer
    {
        //IPropertyContainer _payload;
        //public event ADelegate AEvent
        //{
        //    add
        //    {
        //        _payload.AEvent += value;
        //    }

        //    remove
        //    {
        //        _payload.AEvent -= value;
        //    }
        //}

        public event ADelegate AEvent;
        public event BDelegate BEvent;
        public event CDelegate CEvent;
    }

    public partial class EventContainerProxy : IFakeProxy<IEventContainer>
    {
        public event ADelegate AEvent;
    }

}
