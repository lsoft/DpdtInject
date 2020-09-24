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

namespace DpdtInject.Tests.Singleton.UnknownRequest
{
    public partial class SingletonUnknownRequestModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class SingletonUnknownRequestModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<SingletonUnknownRequestModule>();

                try
                {
                    var b = module.Get<IB>();

                    Assert.Fail("This line should never be executed.");
                }
                catch (InvalidCastException excp)
                {
                    //it's ok, this test is green
                }

                try
                {
                    var b = module.GetAll<IB>();

                    Assert.Fail("This line should never be executed.");
                }
                catch (InvalidCastException excp)
                {
                    //it's ok, this test is green
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
