using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Settings.Constructor.SubsetNoOrder.Two
{
    public partial class SettingsConstructorSubsetNoOrderTwo_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<A>()
                .To<A>()
                .WithTransientScope()
                .Setup<SubsetNoOrderConstructorSetting<int, int, long>>()
                .Configure(new ConstructorArgument("myint0", 1))
                ;
        }

        public class SettingsConstructorSubsetNoOrderTwo_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<SettingsConstructorSubsetNoOrderTwo_Cluster>(
                    null
                    );

                var a = cluster.Get<A>();
                Assert.IsNotNull(a);
                Assert.AreEqual(3, a.ConstructorNumber);
            }
        }
    }


    public class A
    {
        public int ConstructorNumber
        {
            get;
        }

        public A()
        {
            ConstructorNumber = 0;
        }

        public A(long mylong0, long mylong1, int myint0 = 0)
        {
            ConstructorNumber = 1;
        }

        public A(int myint0, int myint1, int myint2)
        {
            ConstructorNumber = 2;
        }

        public A(int myint0, long mylong0 = 0, int myint1 = 0)
        {
            ConstructorNumber = 3;
        }
    }
}
