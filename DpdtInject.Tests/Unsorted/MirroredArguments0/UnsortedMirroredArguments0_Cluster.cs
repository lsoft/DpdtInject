using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;
using DpdtInject.Injector.Src.Bind.Settings.Constructor;
using DpdtInject.Injector.Src.Bind;

namespace DpdtInject.Tests.Unsorted.MirroredArguments0
{
    public partial class UnsortedMirroredArguments0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<MyClass>()
                .To<MyClass>()
                .WithSingletonScope()
                .Setup<AllAndOrderConstructorSetting<short, int>>()
                .Configure(new ConstructorArgument("a", 10))
                .Configure(new ConstructorArgument("b", 100))
                ;
        }

        public class UnsortedMirroredArguments0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedMirroredArguments0_Cluster>(
                    null
                    );

                var myclass = cluster.Get<MyClass>();
                Assert.IsNotNull(myclass);
                Assert.AreEqual(10, myclass.A);
                Assert.AreEqual(100, myclass.B);
            }
        }
    }


    public class MyClass
    {
        public int A
        {
            get;
        }
        public int B
        {
            get;
        }

        public MyClass(
            short a, int b
            )
        {
            A = a;
            B = b;
        }

        public MyClass(
            int a, short b
            )
        {
            A = a;
            B = b;
        }
    }
}
