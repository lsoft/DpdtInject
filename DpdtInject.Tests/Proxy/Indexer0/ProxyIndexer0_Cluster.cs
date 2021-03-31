﻿using System;
using System.Threading;
using DpdtInject.Injector;
using DpdtInject.Injector.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.RContext;
using DpdtInject.Injector.Bind.Settings;

namespace DpdtInject.Tests.Proxy.Indexer0
{
    public partial class ProxyIndexer0_Cluster : DefaultCluster
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
                .When(rt => rt.WhenInjectedExactlyInto<ProxyCalculator>())
                ;

            Bind<IPropertyContainer>()
                .ToProxy<ProxyCalculator>()
                .WithProxySettings<TelemetryAttribute, SessionSaver>()
                .WithSingletonScope()
                .Setup<SuppressCircularCheck>()
                .When(rt => rt.WhenInjectedExactlyNotInto<ProxyCalculator>())
                ;

        }

        public class ProxyIndexer0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ProxyIndexer0_Cluster>(
                    null
                    );

                var propertyContainer = cluster.Get<IPropertyContainer>();
                Assert.IsNotNull(propertyContainer);

                propertyContainer[(short)2] = 2;
                Assert.IsFalse(SessionSaver.ProxyWasInDeal);

                SessionSaver.ProxyWasInDeal = false;
                Assert.AreEqual(2L, propertyContainer[(short)2]);
                Assert.IsFalse(SessionSaver.ProxyWasInDeal);

                SessionSaver.ProxyWasInDeal = false;
                propertyContainer[(int)3, (int)4] = 7;
                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(PropertyContainer).FullName, SessionSaver.FullClassName);
                //SessionSaver.MemberName is too complicated to check it
                Assert.IsTrue(SessionSaver.TakenInSeconds > 0.0);
                Assert.IsNull(SessionSaver.Exception);
                Assert.IsNull(SessionSaver.Arguments);

                SessionSaver.ProxyWasInDeal = false;
                Assert.AreEqual(7L, propertyContainer[(int)3, (int)4]);
                Assert.IsTrue(SessionSaver.ProxyWasInDeal);
                Assert.AreEqual(typeof(PropertyContainer).FullName, SessionSaver.FullClassName);
                //SessionSaver.MemberName is too complicated to check it
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

    public interface IPropertyContainer
    {
        long this[byte i]
        {
            get;
            set;
        }

        long this[short i]
        {
            get;
            set;
        }

        [Telemetry]
        long this[int i, int j]
        {
            get;
            set;
        }
    }

    public class PropertyContainer : IPropertyContainer
    {
        public long this[byte i]
        {
            get => i;
            set { }
        }

        public long this[short i]
        {
            get => i;
            set { }
        }


        public long this[int i, int j]
        {
            get => i + j;
            set { }
        }
    }

    public partial class ProxyCalculator : IFakeProxy<IPropertyContainer>
    {
        public long this[byte i]
        {
            get => i;
            set { }
        }
    }

}
