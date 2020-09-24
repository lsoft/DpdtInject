using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Constant.Hierarchy2WithSame
{
    public partial class ConstantHierarchy2WithSameModule : DpdtModule
    {
        public static A ConstantA1 = new A();
        public static A ConstantA2 = new A();

        public override void Load()
        {
            Bind<IA>()
                .WithConstScope(ConstantA1)
                .InCluster<DefaultCluster>()
                .When(rc => rc.ParentFrame.TargetType == typeof(B1))
                ;

            Bind<IA>()
                .WithConstScope(ConstantA2)
                .InCluster<DefaultCluster>()
                .When(rc => rc.ParentFrame.TargetType == typeof(B2))
                ;

            Bind<IB1>()
                .To<B1>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;

            Bind<IB2>()
                .To<B2>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ConstantHierarchy2WithSameModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConstantHierarchy2WithSameModule>();

                var b1 = module.Get<IB1>();
                Assert.IsNotNull(b1);
                Assert.AreSame(ConstantA1, b1.A);

                var b2 = module.Get<IB2>();
                Assert.IsNotNull(b2);
                Assert.AreSame(ConstantA2, b2.A);
            }
        }

    }


    public interface IA
    {
    }

    public class A : IA
    {
    }

    public interface IB1
    {
        IA A { get; }
    }

    public class B1 : IB1
    {
        public IA A { get; }

        public B1(IA a)
        {
            A = a;
        }
    }

    public interface IB2
    {
        IA A { get; }
    }

    public class B2 : IB2
    {
        public IA A { get; }

        public B2(IA a)
        {
            A = a;
        }
    }
}
