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

namespace DpdtInject.Tests.Unsorted.TooManyChildren
{
    public partial class UnsortedTooManyChildrenModule : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .When(rc => true)
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

        public class UnsortedTooManyChildrenModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedTooManyChildrenModule>();

                try
                {
                    var b = module.Get<IB>();

                    Assert.Fail("this line should never be executed");
                }
                catch (DpdtException excp)
                when (excp.Type == DpdtExceptionTypeEnum.DuplicateBinding && excp.AdditionalArgument == typeof(IA).FullName)
                {
                    //it's OK, this test is gree
                }
            }
        }

    }


    public interface IA
    {
    }

    public class A2 : IA
    {
    }
    public class A1 : IA
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


}
