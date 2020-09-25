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

namespace DpdtInject.Tests.Unsorted.OptionalArgument0
{
    public partial class UnsortedOptionalArgument0Module : DpdtModule
    {
        public const string DefaultMessage = "default message";
        public const string CustomMessage = "custom message";

        public override void Load()
        {

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                .Configure(new ConstructorArgument("message", CustomMessage))
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class UnsortedOptionalArgument0ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedOptionalArgument0Module>();

                var a = module.Get<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(CustomMessage, a.Message);
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

        public A(string message = UnsortedOptionalArgument0Module.DefaultMessage)
        {
            Message = message;
        }

    }

}
