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

namespace DpdtInject.Tests.Conditional.Hierarchy3First
{
    public partial class ConditionalHierarchy3FirstModule : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .Configure(new ConstructorArgument("message", Message))
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ConditionalHierarchy3FirstModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConditionalHierarchy3FirstModule>();

                var c0 = module.Get<IC>();
                Assert.IsNotNull(c0);
                Assert.IsNotNull(c0.B);
                Assert.AreEqual(Message, c0.B.Message);
                Assert.IsNotNull(c0.B.A);

                var cc = module.GetAll<IC>();
                Assert.IsNotNull(cc);
                Assert.AreEqual(1, cc.Count);
                Assert.AreSame(c0, cc[0]);
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
        string Message { get; }

        IA A { get; }
    }

    public class B : IB
    {
        public string Message { get; }
        public IA A { get; }

        public B(string message, IA a)
        {
            Message = message;
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
