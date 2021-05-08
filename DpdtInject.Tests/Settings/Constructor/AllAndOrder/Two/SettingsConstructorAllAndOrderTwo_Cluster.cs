using Microsoft.VisualStudio.TestTools.UnitTesting;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Settings.Constructor.AllAndOrder.Two
{
    public partial class SettingsConstructorAllAndOrderTwo_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {

            Bind<B>()
                .To<B>()
                .WithTransientScope()
                .Setup<AllAndOrderConstructorSetting<int, int>>()
                .Configure(new ConstructorArgument("arg1", 1))
                .Configure(new ConstructorArgument("arg2", 1))
                ;
        }

        public class SettingsConstructorAllAndOrderTwo_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<SettingsConstructorAllAndOrderTwo_Cluster>(
                    null
                    );

                var b = cluster.Get<B>();
                Assert.IsNotNull(b);
                Assert.AreEqual(6, b.ConstructorNumber);
            }
        }
    }


    public class B
    {
        public int ConstructorNumber
        {
            get;
        }

        public B()
        {
            ConstructorNumber = 0;
        }

        public B(int arg1)
        {
            ConstructorNumber = 1;
        }

        public B(long arg1)
        {
            ConstructorNumber = 2;
        }

        public B(int arg1, long arg2)
        {
            ConstructorNumber = 3;
        }

        public B(long arg1, int arg2)
        {
            ConstructorNumber = 5;
        }

        public B(int arg1 = 0, int arg2 = 1)
        {
            ConstructorNumber = 6;
        }
    }
}
