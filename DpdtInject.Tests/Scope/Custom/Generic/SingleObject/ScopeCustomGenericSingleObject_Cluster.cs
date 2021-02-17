using System;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Custom.Generic.SingleObject
{
    public partial class ScopeCustomGenericSingleObject_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                ;
        }

        public class ScopeCustomGenericSingleObject_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ScopeCustomGenericSingleObject_Cluster>(
                    null
                    );

                try
                {
                    var a0 = cluster.Get<IA>();

                    Assert.Fail("This line should never be executed");
                }
                catch(DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.CustomScopeObjectDoesNotFound && excp.AdditionalArgument == typeof(A).FullName)
                {
                    //this is ok, test is green
                }

                var scope1 = cluster.CreateCustomScope();
                var scope2 = cluster.CreateCustomScope();

                var a1 = cluster.Get<IA>(scope1);
                Assert.IsNotNull(a1);
                Assert.IsFalse(a1.Disposed);

                var a2 = cluster.Get<IA>(scope2);
                Assert.IsNotNull(a2);
                Assert.IsFalse(a2.Disposed);

                Assert.AreNotSame(a1, a2);

                var a1_2 = cluster.Get<IA>(scope1);
                Assert.IsNotNull(a1_2);
                Assert.AreSame(a1, a1_2);

                scope1.Dispose();
                Assert.IsTrue(a1.Disposed);
                Assert.IsFalse(a2.Disposed);

                scope2.Dispose();
                Assert.IsTrue(a2.Disposed);
            }
        }

    }


    public interface IA
    {
        bool Disposed
        {
            get;
        }
    }

    public class A : IA, IDisposable
    {
        public bool Disposed
        { 
            get; 
            private set; 
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }

}
