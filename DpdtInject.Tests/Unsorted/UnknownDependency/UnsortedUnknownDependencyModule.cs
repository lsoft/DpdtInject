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
using DpdtInject.Injector.Excp;

namespace DpdtInject.Tests.Unsorted.UnknownDependency
{
    public partial class UnsortedUnknownDependencyModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;

            Bind<IC>()
                .To<C>()
                .WithSingletonScope()
                ;
        }

        public class UnsortedUnknownDependencyModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedUnknownDependencyModule>();

                //try
                //{
                //    var b = module.Get<IB>();

                //    Assert.Fail("this line should never be executed");
                //}
                //catch (DpdtException excp)
                //when (excp.Type == DpdtExceptionTypeEnum.DuplicateBinding && excp.AdditionalArgument == typeof(IA).FullName)
                //{
                //    //it's OK, this test is gree
                //}
            }
        }

    }


    public interface IA
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
