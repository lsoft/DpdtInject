using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DpdtInject.Tests.Conventions.ToSingleInterface0
{
    public partial class ConventionsToSingleInterface0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            ScanInAssembliesWith<A0>()
                .SelectAllWith<IA>()
                .FromAllInterfaces()
                .To<IA>()
                .WithSingletonScope()
                ;
        }

        public class ConventionsToSingleInterface0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConventionsToSingleInterface0_Cluster>(
                    null
                    );

                var a = cluster.GetAll<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(3, a.Count);
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A0)));
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A1)));
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A2)));
            }
        }
    }


    public interface IA
    {
    }

    public class A0 : IA
    {
    }

    public class A1 : IA
    {
    }

    public class A2 : IA
    {
    }
}
