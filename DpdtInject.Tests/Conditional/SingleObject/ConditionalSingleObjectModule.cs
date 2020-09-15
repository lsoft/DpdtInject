using DpdtInject.Injector;
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
                .When()
                ;

            Bind<IA2>()
                .To<A>()
                .WithSingletonScope()
                .When()
                ;
        }

        public class ConditionalSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ConditionalSingleObjectModule>();

                var a1 = module.Get<IA1>();
                Assert.IsNotNull(a1);

                //var a2 = module.Get<IA1>();
                //Assert.IsNotNull(a2);

                //Assert.AreNotSame(a1, a2);
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
