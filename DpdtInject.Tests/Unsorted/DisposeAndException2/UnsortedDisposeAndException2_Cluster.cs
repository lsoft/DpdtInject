using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Unsorted.DisposeAndException2
{
    public partial class UnsortedDisposeAndException2_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<Class1>()
                .To<Class1>()
                .WithSingletonScope()
                ;

            Bind<Class2>()
                .To<Class2>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedDisposeAndException2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedDisposeAndException2_Cluster>(
                    null
                    );

                var class1 = cluster.GetFast(default(Class1));
                Assert.IsFalse(class1.Disposed);

                var class2 = cluster.GetFast(default(Class2));
                Assert.IsFalse(class2.Disposed);

                try
                {
                    cluster.Dispose();

                    Assert.Fail("this line should never be executed");
                }
                catch (AggregateException)
                {
                    //it's ok
                }

                var dclass1 = cluster.GetFast(default(Class1));
                Assert.IsTrue(dclass1.Disposed);

                var dclass2 = cluster.GetFast(default(Class2));
                Assert.IsTrue(dclass2.Disposed);
            }
        }
    }

    public class Class1 : IDisposable
    {
        public bool Disposed; 
        
        public void Dispose()
        {
            Disposed = true;

            throw new InvalidOperationException();
        }
    }

    public class Class2 : IDisposable
    {
        public bool Disposed;

        public void Dispose()
        {
            Disposed = true;

            throw new InvalidOperationException();
        }
    }
}
