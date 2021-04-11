using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.GenericInterface
{
    public partial class UnsortedGenericInterface_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<To<string>>()
                .To<ToIsolatedFactory<int>>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedGenericInterface_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedGenericInterface_Cluster>(
                    null
                    );

                var a = cluster.Get<To<string>>();
                Assert.IsNotNull(a);
            }
        }
    }


    /// <summary>
    /// Keep its name! this name should be equal "To"
    /// for an additional parser's test
    /// </summary>
    public abstract class To<T>
    {
    }

    /// <summary>
    /// Keep its name! this name should be equal "ToIsolatedFactory"
    /// for an additional parser's test
    /// </summary>
    public class ToIsolatedFactory<T> : To<string>
    {
    }
}
