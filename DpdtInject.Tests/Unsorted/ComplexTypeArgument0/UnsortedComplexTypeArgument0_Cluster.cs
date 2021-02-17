using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Unsorted.ComplexTypeArgument0
{
    public partial class UnsortedComplexTypeArgument0_Cluster : DefaultCluster
    {
        public static readonly Func<Func<object>, Func<string>> Funca =
            fo => new Func<string>(() => fo().ToString());

        public override void Load()
        {
            Bind<Func<Func<object>, Func<string>>>()
                .WithConstScope(Funca)
                ;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedComplexTypeArgument0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedComplexTypeArgument0_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(Payload.PayloadString, a.Message);
            }
        }

    }



    public interface IA
    {
        string Message { get; }
    }

    public class A : IA
    {
        public string Message { get; }

        public A(
            Func<Func<object>, Func<string>> funca
            )
        {
            Message = funca(() => new Payload())();
        }

    }

    public class Payload
    {
        public const string PayloadString = "payload string";

        public override string ToString()
        {
            return PayloadString;
        }
    }

}
