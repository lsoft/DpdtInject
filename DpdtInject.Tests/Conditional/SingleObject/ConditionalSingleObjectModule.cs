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

namespace DpdtInject.Tests.Conditional.SingleObject
{
    public partial class ConditionalSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA1>()
                .To<A>()
                .WithSingletonScope()
                .When(rc => true)
                ;

            Bind<IA2>()
                .To<A>()
                .WithSingletonScope()
                .When(rc => false)
                ;
        }

        public class ConditionalSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConditionalSingleObjectModule>();

                var a1 = module.Get<IA1>();
                Assert.IsNotNull(a1);

                var a1s = module.GetAll<IA1>();
                Assert.IsNotNull(a1s);
                Assert.AreEqual(1, a1s.Count);
                Assert.AreSame(a1, a1s[0]);

                try
                {
                    var a2 = module.Get<IA2>();

                    Assert.Fail("test fails, this line should never be executed");
                }
                catch (DpdtException excp)
                    when(excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IA2).FullName)
                {
                    //it's ok
                }

                var a2s = module.GetAll<IA2>();
                Assert.IsNotNull(a2s);
                Assert.AreEqual(0, a2s.Count);

            }
        }

    }

    public interface IA1
    {
    }

    public interface IA2
    {
    }

    public class A : IA1, IA2
    {
    }

}
