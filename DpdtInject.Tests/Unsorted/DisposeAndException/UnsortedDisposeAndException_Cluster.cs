using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace DpdtInject.Tests.Unsorted.DisposeAndException
{
    public partial class UnsortedDisposeAndException_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            Bind<BaseClass>()
                .To<BaseClass>()
                .WithSingletonScope()
                ;

            Bind<DerivedClass>()
                .To<DerivedClass>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedDisposeAndException_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<UnsortedDisposeAndException_Cluster>(
                    null
                    );

                var baseClass0 = cluster.GetFast(default(BaseClass));
                Assert.IsFalse(baseClass0.Disposed);

                try
                {
                    cluster.GetFast(default(DerivedClass));

                    Assert.Fail("this line should never be executed");
                }
                catch (InvalidOperationException)
                {
                    //it's OK
                }

                var baseClass1 = cluster.GetFast(default(BaseClass));
                Assert.AreSame(baseClass0, baseClass1);
                Assert.IsFalse(baseClass1.Disposed);

                cluster.Dispose();
                Assert.IsTrue(baseClass1.Disposed);
            }
        }
    }

    public class BaseClass : IDisposable
    {
        public bool Disposed; 
        
        public void Dispose()
        {
            Disposed = true;
        }
    }

    public class DerivedClass
    {
        public DerivedClass(BaseClass bc)
        {
            throw new InvalidOperationException();
        }
    }
}
