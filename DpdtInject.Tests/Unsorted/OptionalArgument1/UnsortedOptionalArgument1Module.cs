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

namespace DpdtInject.Tests.Unsorted.OptionalArgument1
{
    public partial class UnsortedOptionalArgument1Module : DpdtModule
    {
        public const string DefaultMessage = "default message";

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

        public class UnsortedOptionalArgument1ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedOptionalArgument1Module>();

                var a = module.Get<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(DefaultMessage, a.Message);
            }
        }

    }


    public interface IA
    {
        string Message { get; }
    }

    public class A : IA
    {
        public string Message { get; }

        public A(string message = UnsortedOptionalArgument1Module.DefaultMessage)
        {
            Message = message;
        }

    }

}
