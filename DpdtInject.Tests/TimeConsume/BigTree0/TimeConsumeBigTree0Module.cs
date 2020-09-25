using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DpdtInject.Tests.TimeConsume.BigTree0
{
    public partial class TimeConsumeBigTree0Module : DpdtModule
    {
        public override void Load()
        {
            Bind<IInterface0>().To<Class0>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface1>().To<Class1>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface2>().To<Class2>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface3>().To<Class3>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface4>().To<Class4>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface5>().To<Class5>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface6>().To<Class6>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface7>().To<Class7>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface8>().To<Class8>().WithSingletonScope().InCluster<DefaultCluster>();
            Bind<IInterface9>().To<Class9>().WithSingletonScope().InCluster<DefaultCluster>();
        }

        public partial class DefaultCluster
        {
        }

        public class TimeConsumeBigTree0ModuleTester
        {
            public void PerformModuleTesting()
            {
                var module = new FakeModule<TimeConsumeBigTree0Module>();
            /*
                
{
    var resolvedInstance = module.Get<IInterface0>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface1>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface2>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface3>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface4>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface5>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface6>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface7>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface8>();
    Assert.IsNotNull(resolvedInstance);
}


{
    var resolvedInstance = module.Get<IInterface9>();
    Assert.IsNotNull(resolvedInstance);
}


//*/
            }
        }
    }

    public interface IInterface0
    {
    }

    public class Class0 : IInterface0
    {
        public IInterface1 Argument1
        {
            get;
        }

        public IInterface4 Argument4
        {
            get;
        }

        public Class0(IInterface1 argument1, IInterface4 argument4)
        {
            Argument1 = argument1;
            Argument4 = argument4;
        }
    }

    public interface IInterface1
    {
    }

    public class Class1 : IInterface1
    {
        public Class1()
        {
        }
    }

    public interface IInterface2
    {
    }

    public class Class2 : IInterface2
    {
        public IInterface3 Argument3
        {
            get;
        }

        public IInterface8 Argument8
        {
            get;
        }

        public Class2(IInterface3 argument3, IInterface8 argument8)
        {
            Argument3 = argument3;
            Argument8 = argument8;
        }
    }

    public interface IInterface3
    {
    }

    public class Class3 : IInterface3
    {
        public Class3()
        {
        }
    }

    public interface IInterface4
    {
    }

    public class Class4 : IInterface4
    {
        public IInterface6 Argument6
        {
            get;
        }

        public IInterface7 Argument7
        {
            get;
        }

        public Class4(IInterface6 argument6, IInterface7 argument7)
        {
            Argument6 = argument6;
            Argument7 = argument7;
        }
    }

    public interface IInterface5
    {
    }

    public class Class5 : IInterface5
    {
        public IInterface9 Argument9
        {
            get;
        }

        public Class5(IInterface9 argument9)
        {
            Argument9 = argument9;
        }
    }

    public interface IInterface6
    {
    }

    public class Class6 : IInterface6
    {
        public Class6()
        {
        }
    }

    public interface IInterface7
    {
    }

    public class Class7 : IInterface7
    {
        public Class7()
        {
        }
    }

    public interface IInterface8
    {
    }

    public class Class8 : IInterface8
    {
        public Class8()
        {
        }
    }

    public interface IInterface9
    {
    }

    public class Class9 : IInterface9
    {
        public Class9()
        {
        }
    }
}