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

namespace DpdtInject.Tests.Unsorted.ComplexTypeArgument0
{
    public partial class UnsortedComplexTypeArgument0Module : DpdtModule
    {
        public static Func<Func<object>, Func<string>> Funca =>
            fo => new Func<string>( () => fo().ToString() );

        public override void Load()
        {
            Bind<Func<Func<object>, Func<string>>>()
                .WithConstScope(Funca)
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

        public class UnsortedComplexTypeArgument0ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<UnsortedComplexTypeArgument0Module>();

                var a = module.Get<IA>();
                Assert.IsNotNull(a);
                Assert.AreEqual(Payload.PayloadString, a.Message);
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

        public A(
            Func<Func<object>, Func<string>> funca
            )
        {
            Message = funca(() => new Payload())();
        }

    }

    public class Payload
    {
        public const string PayloadString = "payload string";

        public override string ToString()
        {
            return PayloadString;
        }
    }

}
