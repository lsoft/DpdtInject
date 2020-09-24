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
using System.ComponentModel.DataAnnotations;

namespace DpdtInject.Tests.Conditional.OnlyOne
{
    public partial class ConditionalOnlyOneModule : DpdtModule
    {
        public const string Message = "some message";

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
                .When(rc => false)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ConditionalOnlyOneModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConditionalOnlyOneModule>();

                var c0 = module.Get<IC>();
                Assert.IsNotNull(c0);
                Assert.IsNotNull(c0.B);
                Assert.IsNotNull(c0.B.A);
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

        public B(IA a)
        {
            A = a;
        }
    }

    public interface IC
    {
        IB B { get; }
    }

    public class C : IC
    {
        public IB B { get; }

        public C(IB b)
        {
            B = b;
        }
    }

}
