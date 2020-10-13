//seed: -624853138
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0
{
    public partial class TimeConsumeBigTree0_Cluster : DefaultCluster
    {
        public const int BindCount = 10;
        public const string BindCountString = "10";
        public const string GenericTestName = "GenericSingleton10";
        public const string NonGenericTestName = "NonGenericSingleton10";
        public const string FastTestName = "FastSingleton10";
        public override void Load()
        {
#region bind code
            Bind<IInterface0>().To<Class0>().WithSingletonScope();
            Bind<IInterface1>().To<Class1>().WithSingletonScope();
            Bind<IInterface2>().To<Class2>().WithSingletonScope();
            Bind<IInterface3>().To<Class3>().WithSingletonScope();
            Bind<IInterface4>().To<Class4>().WithSingletonScope();
            Bind<IInterface5>().To<Class5>().WithSingletonScope();
            Bind<IInterface6>().To<Class6>().WithSingletonScope();
            Bind<IInterface7>().To<Class7>().WithSingletonScope();
            Bind<IInterface8>().To<Class8>().WithSingletonScope();
            Bind<IInterface9>().To<Class9>().WithSingletonScope();
            ;
#endregion
        }

#region resolution code
        public static void ResolveGeneric(TimeConsumeBigTree0_Cluster cluster)
        {
            var resolvedInstance0 = cluster.Get<IInterface0>();
            var resolvedInstance1 = cluster.Get<IInterface1>();
            var resolvedInstance2 = cluster.Get<IInterface2>();
            var resolvedInstance3 = cluster.Get<IInterface3>();
            var resolvedInstance4 = cluster.Get<IInterface4>();
            var resolvedInstance5 = cluster.Get<IInterface5>();
            var resolvedInstance6 = cluster.Get<IInterface6>();
            var resolvedInstance7 = cluster.Get<IInterface7>();
            var resolvedInstance8 = cluster.Get<IInterface8>();
            var resolvedInstance9 = cluster.Get<IInterface9>();
            ;
        }

        public static void ResolveNonGeneric(TimeConsumeBigTree0_Cluster cluster)
        {
            var resolvedInstance0 = cluster.Get(typeof(IInterface0));
            var resolvedInstance1 = cluster.Get(typeof(IInterface1));
            var resolvedInstance2 = cluster.Get(typeof(IInterface2));
            var resolvedInstance3 = cluster.Get(typeof(IInterface3));
            var resolvedInstance4 = cluster.Get(typeof(IInterface4));
            var resolvedInstance5 = cluster.Get(typeof(IInterface5));
            var resolvedInstance6 = cluster.Get(typeof(IInterface6));
            var resolvedInstance7 = cluster.Get(typeof(IInterface7));
            var resolvedInstance8 = cluster.Get(typeof(IInterface8));
            var resolvedInstance9 = cluster.Get(typeof(IInterface9));
            ;
        }

        public static void ResolveFast(TimeConsumeBigTree0_Cluster cluster)
        {
            var resolvedInstance0 = cluster.GetFast(default(IInterface0));
            var resolvedInstance1 = cluster.GetFast(default(IInterface1));
            var resolvedInstance2 = cluster.GetFast(default(IInterface2));
            var resolvedInstance3 = cluster.GetFast(default(IInterface3));
            var resolvedInstance4 = cluster.GetFast(default(IInterface4));
            var resolvedInstance5 = cluster.GetFast(default(IInterface5));
            var resolvedInstance6 = cluster.GetFast(default(IInterface6));
            var resolvedInstance7 = cluster.GetFast(default(IInterface7));
            var resolvedInstance8 = cluster.GetFast(default(IInterface8));
            var resolvedInstance9 = cluster.GetFast(default(IInterface9));
            ;
        }
#endregion
    }
}