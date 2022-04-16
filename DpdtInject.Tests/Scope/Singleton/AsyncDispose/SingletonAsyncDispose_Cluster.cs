using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Scope.Singleton.AsyncDispose
{
    public partial class SingletonAsyncDispose_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }


        public class SingletonAsyncDispose_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<SingletonAsyncDispose_Cluster>(
                    null
                    );

                var a0 = cluster.Get<IA>();
                Assert.IsNotNull(a0);

                Assert.AreEqual(0, A.DisposeCount);

                cluster.Dispose();

                Assert.AreEqual(0, A.DisposeCount);

                cluster.DisposeAsync().GetAwaiter().GetResult();

                Assert.AreEqual(1, A.DisposeCount);
            }
        }
    }


    public interface IA
    {
    }

    public class A : IA, IAsyncDisposable
    {
        public static int DisposeCount = 0;

        public ValueTask DisposeAsync()
        {
            DisposeCount++;

            return ValueTask.CompletedTask;
        }
    }
}
