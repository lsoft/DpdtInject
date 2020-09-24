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

namespace DpdtInject.Tests.Unsorted.CircularDependency3
{
    public partial class UnsortedCircularDependency3Module : DpdtModule
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

        public class UnsortedCircularDependency3ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedCircularDependency3Module>();
            }
        }

    }


    public interface IA
    {
        IC C { get; }
    }

    public class A : IA
    {
        public IC C { get; }

        public A(IC c)
        {
            C = c;
        }
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
