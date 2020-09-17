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

namespace DpdtInject.Tests.Unsorted.LocalVariable
{
    public partial class LocalVariableTransientModule : DpdtModule
    {
        public const string Message = "some message";

        public override void Load()
        {
            var currentMessage = Message;

            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", currentMessage))
                ;
        }

        public class LocalVariableTransientTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<LocalVariableTransientModule>();

                //var a = module.Get<IA>();
                //Assert.IsNotNull(a);
                //Assert.AreEqual(Message, a.Message);
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
