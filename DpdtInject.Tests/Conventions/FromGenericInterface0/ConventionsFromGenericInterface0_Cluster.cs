using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using DpdtInject.Injector.Src;

namespace DpdtInject.Tests.Conventions.FromGenericInterface0
{
    public partial class ConventionsFromGenericInterface0_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            ScanInAssembliesWith<A0>()
                .SelectAllWithOpenGeneric<IA<object>>()
                .ExcludeAllWithOpenGeneric<IExclude<object>>()
                .FromAllInterfaces()
                .ToItself()
                .WithSingletonScope()
                ;
        }

        public class ConventionsFromGenericInterface0_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConventionsFromGenericInterface0_Cluster>(
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

    public interface IExclude<T>
    {
    }

    public interface IA<T> : IA
    {
    }

    public class A0 : IA<object>
    {
    }

    public class A1 : IA<string>, IExclude<ulong>
    {
    }

    public class A2 : IA<StringBuilder>
    {
    }
}
