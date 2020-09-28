using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.RContext;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.Scope.Custom.NonGenericSingleObject
{
    public partial class ScopeCustomNonGenericSingleObjectModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                .InCluster<DefaultCluster>()
                ;
        }

        public partial class DefaultCluster
        {

        }

        public class ScopeCustomNonGenericSingleObjectModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<ScopeCustomNonGenericSingleObjectModule>();

                try
                {
                    var a0 = module.Get(typeof(IA));

                    Assert.Fail("This line should never be executed");
                }
                catch(DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.NoBindingAvailable && excp.AdditionalArgument == typeof(IA).FullName)
                {
                    //this is ok, test is green
                }

                var scope1 = module.CreateCustomScope();
                var scope2 = module.CreateCustomScope();

                var a1 = (IA)module.Get(typeof(IA), scope1);
                Assert.IsNotNull(a1);
                Assert.IsFalse(a1.Disposed);

                var a2 = (IA)module.Get(typeof(IA), scope2);
                Assert.IsNotNull(a2);
                Assert.IsFalse(a2.Disposed);

                Assert.AreNotSame(a1, a2);

                var a1_2 = (IA)module.Get(typeof(IA), scope1);
                Assert.IsNotNull(a1_2);
                Assert.AreSame(a1, a1_2);

                scope1.Dispose();
                Assert.IsTrue(a1.Disposed);
                Assert.IsFalse(a2.Disposed);

                scope2.Dispose();
                Assert.IsTrue(a2.Disposed);
            }
        }

    }


    public interface IA
    {
        bool Disposed
        {
            get;
        }
    }

    public class A : IA, IDisposable
    {
        public bool Disposed
        { 
            get; 
            private set; 
        }

        public void Dispose()
        {
            Disposed = true;
        }
    }

}
