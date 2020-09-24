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

namespace DpdtInject.Tests.Func.Generic.Transient.Hierarchy2
{
    public partial class FuncGenericTransientHierarchy2Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithTransientScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB>()
                .To<B>()
                .WithTransientScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class FuncGenericTransientHierarchy2ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<FuncGenericTransientHierarchy2Module>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);

                var b0 = module.Get<IB>();
                Assert.IsNotNull(b0);
                Assert.IsNotNull(b0.A);
                Assert.AreNotSame(a0, b0.A);
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
