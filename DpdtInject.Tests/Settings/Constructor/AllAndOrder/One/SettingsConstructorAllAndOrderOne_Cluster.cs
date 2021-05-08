using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Settings.Constructor.AllAndOrder.One
{
    public partial class SettingsConstructorAllAndOrderOne_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<A>()
                .To<A>()
                .WithTransientScope()
                .Setup<AllAndOrderConstructorSetting<int, long>>()
                .Configure(new ConstructorArgument("myint", 1))
                ;
        }

        public class SettingsConstructorAllAndOrderOne_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<SettingsConstructorAllAndOrderOne_Cluster>(
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

        public A(int myint)
        {
            ConstructorNumber = 1;
        }

        public A(long mylong)
        {
            ConstructorNumber = 2;
        }

        public A(int myint, long mylong = 0)
        {
            ConstructorNumber = 3;
        }

        public A(long mylong, int myint)
        {
            ConstructorNumber = 4;
        }
    }
}
