﻿using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Tests.Singleton.TwoSameBindings
{
    public partial class SingletonTwoSameBindingsModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA>()
                .To<A1>()
                .WithSingletonScope()
                ;

            Bind<IA>()
                .To<A2>()
                .WithSingletonScope()
                ;
        }

        public class SingletonTwoSameBindingsModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<SingletonTwoSameBindingsModule>();

                var aa = module.GetAll<IA>();
                Assert.IsNotNull(aa);
                Assert.AreEqual(2, aa.Count);
                Assert.AreNotSame(aa[0], aa[1]);

                try
                {
                    var a0 = module.Get<IA>();

                    Assert.Fail("This line should never be executed.");
                }
                catch (DpdtException excp)
                    when (excp.Type == DpdtExceptionTypeEnum.DuplicateBinding && excp.AdditionalArgument == typeof(IA).FullName)
                {
                    //it's ok, this test is green
                }
            }
        }

    }


    public interface IA
    {
    }

    public class A1 : IA
    {
    }

    public class A2 : IA
    {
    }
}