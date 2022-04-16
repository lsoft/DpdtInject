using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using System.Threading.Tasks;
using DpdtInject.Injector.Src.Excp;

namespace DpdtInject.Tests.Scope.Custom.AsyncDispose
{
    public partial class CustomAsyncDispose_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                ;
        }


        public class CustomAsyncDispose_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<CustomAsyncDispose_Cluster>(
                    null
                    );

                try
                {
                    var a0 = cluster.Get<IA>();

                    Assert.Fail("This line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.CustomScopeObjectDoesNotFound && excp.AdditionalArgument == typeof(A).FullName)
                {
                    //this is ok, test is green
                }

                var scope = cluster.CreateCustomScope();

                var a1 = cluster.Get<IA>(scope);
                Assert.IsNotNull(a1);

                Assert.AreEqual(0, A.DisposeCount);

                scope.Dispose();

                Assert.AreEqual(0, A.DisposeCount);

                scope.DisposeAsync().GetAwaiter().GetResult();

                Assert.AreEqual(1, A.DisposeCount);

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
