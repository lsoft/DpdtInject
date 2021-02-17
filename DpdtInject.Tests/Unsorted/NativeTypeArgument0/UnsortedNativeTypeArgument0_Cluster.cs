using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Unsorted.NativeTypeArgument0
{
    public partial class UnsortedNativeTypeArgument0_Cluster : DefaultCluster
    {
        public const string DefaultMessage = "default message";

        public override void Load()
        {
            Bind<string>()
                .WithConstScope(DefaultMessage)
                ;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedNativeTypeArgument0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedNativeTypeArgument0_Cluster>(
                    null
                    );

                var a = cluster.Get<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(DefaultMessage, a.Message);
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

        public A(string message)
        {
            Message = message;
        }

    }

}
