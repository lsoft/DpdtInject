using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Settings.Constructor.SubsetAndOrder.Two
{
    public partial class SettingsConstructorSubsetAndOrderTwo_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<A>()
                .To<A>()
                .WithTransientScope()
                .Setup<SubsetAndOrderConstructorSetting<int, long>>()
                .Configure(new ConstructorArgument("myint0", 1))
                ;
        }

        public class SettingsConstructorSubsetAndOrderTwo_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<SettingsConstructorSubsetAndOrderTwo_Cluster>(
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

        public A(long mylong0, int myint1)
        {
            ConstructorNumber = 2;
        }

        public A(int myint0, long mylong0 = 0, int myint1 = 0)
        {
            ConstructorNumber = 3;
        }

    }
}
