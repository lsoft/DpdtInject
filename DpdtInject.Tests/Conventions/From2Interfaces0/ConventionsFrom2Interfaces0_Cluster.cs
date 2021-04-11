using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Conventions.From2Interfaces0
{
    public partial class ConventionsFrom2Interfaces0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            ScanInAssembliesWith<A0>()
                .SelectAllWith<IA>()
                .From<IA, IB>()
                .ToItself()
                .WithSingletonScope()
                ;
        }

        public class ConventionsFrom2Interfaces0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConventionsFrom2Interfaces0_Cluster>(
                    null
                    );

                var a = cluster.GetAll<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(3, a.Count);
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A0)));
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A1)));
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A2)));

                var b = cluster.GetAll<IB>();
                Assert.IsNotNull(b);
                Assert.AreEqual(3, b.Count);
                Assert.IsTrue(b.Any(bb => bb.GetType() == typeof(A0)));
                Assert.IsTrue(b.Any(bb => bb.GetType() == typeof(A1)));
                Assert.IsTrue(b.Any(bb => bb.GetType() == typeof(A2)));

                foreach (var pair in a.OrderBy(t => t.GetType().Name).Zip(b.OrderBy(t => t.GetType().Name)))
                {
                    Assert.IsTrue(ReferenceEquals(pair.First, pair.Second));
                }
            }
        }
    }


    public interface IA
    {
    }

    public interface IB
    {
    }

    public class A0 : IA, IB
    {
    }

    public class A1 : IA, IB
    {
    }

    public class A2 : IA, IB
    {
    }
}
