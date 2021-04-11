using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src.Bind.Settings.Circular;
using DpdtInject.Injector.Src.Bind;
using DpdtInject.Injector.Src.RContext;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Proxy.Property0
{
    public partial class ProxyProperty0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<SessionSaver>()
                .To<SessionSaver>()
                .WithSingletonScope()
                ;

            Bind<IPropertyContainer>()
                .To<PropertyContainer>()
                .WithSingletonScope()
                .When(rt => rt.WhenInjectedExactlyInto<PropertyContainerProxy>())
                ;

            Bind<IPropertyContainer>()
                .ToProxy<PropertyContainerProxy>()
                .WithProxySettings<TelemetryAttribute, SessionSaver>()
                .WithSingletonScope()
                .Setup<SuppressCircularCheck>()
                .When(rt => rt.WhenInjectedExactlyNotInto<PropertyContainerProxy>())
                ;

        }

        public class ProxyProperty0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ProxyProperty0_Cluster>(
                    null
                    );

                var propertyContainer = cluster.Get<IPropertyContainer>();
                Assert.IsNotNull(propertyContainer);

                propertyContainer.ReadWriteProperty2 = "1";
                Assert.AreEqual("1", propertyContainer.ReadWriteProperty2);

                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(PropertyContainer).FullName, SessionSaver.FullClassName);
                Assert.AreEqual(nameof(PropertyContainer.ReadWriteProperty2), SessionSaver.MemberName);
                Assert.IsTrue(SessionSaver.TakenInSeconds > 0.0);
                Assert.IsNull(SessionSaver.Exception);
                Assert.IsNull(SessionSaver.Arguments);
            }
        }
    }


    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
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


    public interface IPropertyContainer
    {
        string AlreadyImplementedProperty
        {
            get;
            set;
        }

        string ReadProperty
        {
            get;
        }

        string WriteProperty
        {
            set;
        }

        string ReadWriteProperty
        {
            get;
            set;
        }

        [Telemetry]
        string ReadWriteProperty2
        {
            get;
            set;
        }
    }

    public class PropertyContainer : IPropertyContainer
    {
        public string ReadProperty => "You read";

        public string AlreadyImplementedProperty
        {
            get;
            set;
        }
        public string WriteProperty
        {
            set { }
        }
        public string ReadWriteProperty
        {
            get;
            set;
        }

        public string ReadWriteProperty2
        {
            get;
            set;
        }

    }

    public partial class PropertyContainerProxy : IFakeProxy<IPropertyContainer>
    {
        public string AlreadyImplementedProperty
        {
            get;
            set;
        }
    }

}
