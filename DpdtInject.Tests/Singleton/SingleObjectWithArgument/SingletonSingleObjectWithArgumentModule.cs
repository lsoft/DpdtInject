using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Singleton.SingleObjectWithArgument
{
    public partial class SingletonSingleObjectWithArgumentModule : DpdtModule
    {
        public const string Message = "some message";
        public const ulong SecondArgument = 8745uL;

        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .Configure(new ConstructorArgument("message", Message))
                .Configure(new ConstructorArgument("secondArgument", SecondArgument))
                ;
        }

        public class SingletonSingleObjectWithArgumentModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<SingletonSingleObjectWithArgumentModule>();

                var a0 = module.Get<IA>();
                Assert.IsNotNull(a0);
                Assert.AreEqual(Message, a0.Message);
                Assert.AreEqual(SecondArgument, a0.SecondArgument);

                var a1 = module.Get<IA>();
                Assert.IsNotNull(a1);

                Assert.AreSame(a0, a1);

                var aa = module.GetAll<IA>();
                Assert.IsNotNull(aa);
                Assert.AreEqual(1, aa.Count);
                Assert.AreSame(a0, aa[0]);
            }
        }

    }


    public interface IA
    {
        ulong SecondArgument { get; }
        string Message { get; }
    }

    public class A : IA
    {
        public ulong SecondArgument { get; }
        public string Message { get; }

        public A(
            ulong secondArgument,
            string message
            )
        {
            SecondArgument = secondArgument;
            Message = message;
        }

    }

}
