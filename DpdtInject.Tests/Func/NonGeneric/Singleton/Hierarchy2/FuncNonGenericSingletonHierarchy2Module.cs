using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector.Module.Bind;

namespace DpdtInject.Tests.Func.NonGeneric.Singleton.Hierarchy2
{
    public partial class FuncNonGenericSingletonHierarchy2Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {
        }

        public class FuncNonGenericSingletonHierarchy2ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<FuncNonGenericSingletonHierarchy2Module>();

                var a0 = (IA)module.Get(typeof(IA));
                Assert.IsNotNull(a0);

                var b0 = (IB)module.Get(typeof(IB));
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreSame(a0, b0.A);
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
