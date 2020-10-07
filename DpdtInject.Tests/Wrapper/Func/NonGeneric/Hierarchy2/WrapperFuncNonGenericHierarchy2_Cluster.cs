using DpdtInject.Injector;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Wrapper.Func.NonGeneric.Hierarchy2
{
    public partial class WrapperFuncNonGenericHierarchy2_Cluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                ;
        }

        public class WrapperFuncNonGenericHierarchy2_ClusterTester
        {
            public void PerformClusterTesting()
            {
                var cluster = new FakeCluster<WrapperFuncNonGenericHierarchy2_Cluster>(
                    null
                    );

                var a = (IA)cluster.Get(typeof(IA));
                Assert.IsNotNull(a);

                var b0 = (IB)cluster.Get(typeof(IB));
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
            }
        }

    }


    public interface IA
    {

    }

    public class A : IA
    {

    }

    public interface IB
    {
        IA A { get; }
    }

    public class B : IB
    {
        public IA A { get; }

        public B(Func<IA> af)
        {
            A = af();
        }

    }
}
