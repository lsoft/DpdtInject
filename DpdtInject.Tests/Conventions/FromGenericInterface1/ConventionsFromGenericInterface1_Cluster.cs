using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace DpdtInject.Tests.Conventions.FromGenericInterface1
{
    public partial class ConventionsFromGenericInterface1_Cluster : DefaultCluster
    {
        [DpdtBindingMethod]
        public void BindMethod()
        {
            ScanInAssembliesWith<A0>()
                .SelectAllWithOpenGeneric<IA<IEnumerable, Array>>()
                .ExcludeAllWithOpenGeneric<IExclude<object>>()
                .FromAllInterfaces()
                .ToItself()
                .WithSingletonScope()
                ;
        }

        public class ConventionsFromGenericInterface1_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<ConventionsFromGenericInterface1_Cluster>(
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

    public interface IA<T1, T2> : IA
    {
    }

    public class A0 : IA<object, byte>
    {
    }

    public class A1 : IA<string, Encoder>, IExclude<ulong>
    {
    }

    public class A2 : IA<StringBuilder, ushort>
    {
    }
}
