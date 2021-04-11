using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.BindFrom3And4
{
    public partial class UnsortedBindFrom3And4_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<IAInterface0, IAInterface1, IAInterface2>()
                .To<AClass012>()
                .WithSingletonScope()
                ;

            Bind<IBInterface0, IBInterface1, IBInterface2, IBInterface3>()
                .To<BClass0123>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedBindFrom3And4_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedBindFrom3And4_Cluster>(
                    null
                    );

                #region check for 3 bind from

                var aclass012_0 = cluster.GetFast(default(IAInterface0));
                Assert.IsNotNull(aclass012_0);
                var aclass012_1 = cluster.GetFast(default(IAInterface1));
                Assert.IsNotNull(aclass012_1);
                var aclass012_2 = cluster.GetFast(default(IAInterface2));
                Assert.IsNotNull(aclass012_2);

                Assert.AreSame(aclass012_0, aclass012_1);
                Assert.AreSame(aclass012_1, aclass012_2);

                #endregion

                #region check for 4 bind from

                var bclass0123_0 = cluster.GetFast(default(IBInterface0));
                Assert.IsNotNull(bclass0123_0);
                var bclass0123_1 = cluster.GetFast(default(IBInterface1));
                Assert.IsNotNull(bclass0123_1);
                var bclass0123_2 = cluster.GetFast(default(IBInterface2));
                Assert.IsNotNull(bclass0123_2);
                var bclass0123_3 = cluster.GetFast(default(IBInterface3));
                Assert.IsNotNull(bclass0123_3);

                Assert.AreSame(bclass0123_0, bclass0123_1);
                Assert.AreSame(bclass0123_1, bclass0123_2);
                Assert.AreSame(bclass0123_2, bclass0123_3);

                #endregion

            }
        }
    }

    public interface IAInterface0
    {
    }
    public interface IAInterface1
    {
    }
    public interface IAInterface2
    {
    }

    public class AClass012 : IAInterface0, IAInterface1, IAInterface2
    {
    }


    public interface IBInterface0
    {
    }
    public interface IBInterface1
    {
    }
    public interface IBInterface2
    {
    }
    public interface IBInterface3
    {
    }

    public class BClass0123 : IBInterface0, IBInterface1, IBInterface2, IBInterface3
    {
    }
}
