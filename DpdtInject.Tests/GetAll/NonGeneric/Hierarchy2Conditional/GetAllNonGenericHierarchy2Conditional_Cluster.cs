using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.GetAll.NonGeneric.Hierarchy2Conditional
{
    public partial class GetAllNonGenericHierarchy2Conditional_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IZ>()
                .To<Z>()
                .WithSingletonScope()
                .When(rt => rt.IsGetAllResolution)
                ;

            Bind<IA>()
                .To<A0>()
                .WithSingletonScope()
                ;

            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                ;
        }

        public class GetAllNonGenericHierarchy2Conditional_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<GetAllNonGenericHierarchy2Conditional_Cluster>(
                    null
                    );

                var alist = (List<IA>)cluster.GetAll(typeof(IA));
                Assert.IsNotNull(alist);
                Assert.AreEqual(2, alist.Count);
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A0)));
                Assert.IsTrue(alist.Any(a => a.ImplementationType == typeof(A1)));
            }
        }

    }

    public interface IZ
    {

    }

    public class Z : IZ
    {
    }



    public interface IA
    {
        Type ImplementationType
        {
            get;
        }
    }

    public class A0 : IA
    {
        public Type ImplementationType => typeof(A0);

        public IZ Z { get; }

        public A0(IZ z)
        {
            Z = z;
        }
    }

    public class A1 : IA
    {
        public Type ImplementationType => typeof(A1);
    }
}
