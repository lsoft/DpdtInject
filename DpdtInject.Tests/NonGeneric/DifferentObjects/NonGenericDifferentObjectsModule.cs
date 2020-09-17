using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.NonGeneric.DifferentObjects
{
    public partial class NonGenericDifferentObjectsModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                ;

            Bind<IB>()
                .To<B>()
                .WithSingletonScope()
                ;
        }

        public class NonGenericDifferentObjectsModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<NonGenericDifferentObjectsModule>();

                {
                    var a0 = module.Get(typeof(IA));
                    Assert.IsNotNull(a0);
                    Assert.AreEqual(typeof(A), a0.GetType());

                    var a1 = module.Get(typeof(IA));
                    Assert.IsNotNull(a1);
                    Assert.AreEqual(typeof(A), a1.GetType());

                    Assert.AreSame(a0, a1);

                    var aa = module.GetAll(typeof(IA));
                    Assert.IsNotNull(aa);
                    Assert.AreEqual(1, aa.Count);
                    Assert.AreSame(a0, aa[0]);
                }

                {
                    var b0 = module.Get(typeof(IB));
                    Assert.IsNotNull(b0);
                    Assert.AreEqual(typeof(B), b0.GetType());

                    var b11 = module.Get(typeof(IB));
                    Assert.IsNotNull(b11);
                    Assert.AreEqual(typeof(B), b11.GetType());

                    Assert.AreSame(b0, b11);

                    var bb = module.GetAll(typeof(IB));
                    Assert.IsNotNull(bb);
                    Assert.AreEqual(1, bb.Count);
                    Assert.AreSame(b0, bb[0]);
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

    public class B : IB
    {
    }
}
