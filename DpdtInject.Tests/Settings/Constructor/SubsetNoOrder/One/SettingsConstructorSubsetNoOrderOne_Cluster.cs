using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Settings.Constructor.SubsetNoOrder.One
{
    public partial class SettingsConstructorSubsetNoOrderOne_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<A>()
                .To<A>()
                .WithTransientScope()
                .Setup<SubsetNoOrderConstructorSetting<int, long>>()
                .Configure(new ConstructorArgument("mylong", 1))
                ;
        }

        public class SettingsConstructorSubsetNoOrderOne_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<SettingsConstructorSubsetNoOrderOne_Cluster>(
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

        public A(long mylong, int myint = 0)
        {
            ConstructorNumber = 3;
        }
    }
}
