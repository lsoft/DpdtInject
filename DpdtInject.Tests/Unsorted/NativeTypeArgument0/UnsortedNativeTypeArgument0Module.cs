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

namespace DpdtInject.Tests.Unsorted.NativeTypeArgument0
{
    public partial class UnsortedNativeTypeArgument0Module : DpdtModule
    {
        public const string DefaultMessage = "default message";

        public override void Load()
        {
            Bind<string>()
                .WithConstScope(DefaultMessage)
                .InCluster<DefaultCluster>()
                ;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class UnsortedNativeTypeArgument0ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedNativeTypeArgument0Module>();

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

        public A(string message)
        {
            Message = message;
        }

    }

}
