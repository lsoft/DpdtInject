using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.NonGeneric.IncorrectRequest
{
    public partial class NonGenericIncorrectRequestModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;
        }

        public class NonGenericIncorrectRequestModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<NonGenericIncorrectRequestModule>();

                try
                {
                    var r = module.Get(typeof(IB));

                    Assert.Fail("test fails, this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IB).FullName)
                {
                    //it's ok
                }

                try
                {
                    var rs = module.GetAll(typeof(IB));

                    Assert.Fail("test fails, this line should never be executed");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IB).FullName)
                {
                    //it's ok
                }
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
    }
}
