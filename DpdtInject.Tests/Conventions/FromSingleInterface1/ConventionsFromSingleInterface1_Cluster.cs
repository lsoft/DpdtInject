using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DpdtInject.Tests.Conventions.FromSingleInterface1
{
    public partial class ConventionsFromSingleInterface1_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            ScanInAssembliesWith<A0>()
                .SelectAllWith<IA>()
                .ExcludeAllWith<IExclude>()
                .From<IA>()
                .ToItself()
                .WithSingletonScope()
                ;
        }

        public class ConventionsFromSingleInterface1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConventionsFromSingleInterface1_Cluster>(
                    null
                    );

                var a = cluster.GetAll<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(2, a.Count);
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A0)));
                Assert.IsTrue(a.Any(aa => aa.GetType() == typeof(A2)));
            }
        }
    }


    public interface IA
    {
    }

    public interface IExclude
    {
    }

    public class A0 : IA
    {
    }

    public class A1 : IA, IExclude
    {
    }

    public class A2 : IA
    {
    }
}
