//seed: -624853138
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using MicroResolver;

namespace DpdtInject.Tests.Performance.TimeConsume.BigTree0
{
    public static class MicroResolverRelated
    {
        public const int BindCount = 50;
        public const string BindCountString = "50";
        public const string GenericTestName = "GenericSingleton50";
        public const string NonGenericTestName = "NonGenericSingleton50";
        public static void Bind(ObjectResolver container)
        {
#region bind code
            container.Register<IInterface0, Class0>(Lifestyle.Singleton);
            container.Register<IInterface1, Class1>(Lifestyle.Singleton);
            container.Register<IInterface2, Class2>(Lifestyle.Singleton);
            container.Register<IInterface3, Class3>(Lifestyle.Singleton);
            container.Register<IInterface4, Class4>(Lifestyle.Singleton);
            container.Register<IInterface5, Class5>(Lifestyle.Singleton);
            container.Register<IInterface6, Class6>(Lifestyle.Singleton);
            container.Register<IInterface7, Class7>(Lifestyle.Singleton);
            container.Register<IInterface8, Class8>(Lifestyle.Singleton);
            container.Register<IInterface9, Class9>(Lifestyle.Singleton);
            container.Register<IInterface10, Class10>(Lifestyle.Singleton);
            container.Register<IInterface11, Class11>(Lifestyle.Singleton);
            container.Register<IInterface12, Class12>(Lifestyle.Singleton);
            container.Register<IInterface13, Class13>(Lifestyle.Singleton);
            container.Register<IInterface14, Class14>(Lifestyle.Singleton);
            container.Register<IInterface15, Class15>(Lifestyle.Singleton);
            container.Register<IInterface16, Class16>(Lifestyle.Singleton);
            container.Register<IInterface17, Class17>(Lifestyle.Singleton);
            container.Register<IInterface18, Class18>(Lifestyle.Singleton);
            container.Register<IInterface19, Class19>(Lifestyle.Singleton);
            container.Register<IInterface20, Class20>(Lifestyle.Singleton);
            container.Register<IInterface21, Class21>(Lifestyle.Singleton);
            container.Register<IInterface22, Class22>(Lifestyle.Singleton);
            container.Register<IInterface23, Class23>(Lifestyle.Singleton);
            container.Register<IInterface24, Class24>(Lifestyle.Singleton);
            container.Register<IInterface25, Class25>(Lifestyle.Singleton);
            container.Register<IInterface26, Class26>(Lifestyle.Singleton);
            container.Register<IInterface27, Class27>(Lifestyle.Singleton);
            container.Register<IInterface28, Class28>(Lifestyle.Singleton);
            container.Register<IInterface29, Class29>(Lifestyle.Singleton);
            container.Register<IInterface30, Class30>(Lifestyle.Singleton);
            container.Register<IInterface31, Class31>(Lifestyle.Singleton);
            container.Register<IInterface32, Class32>(Lifestyle.Singleton);
            container.Register<IInterface33, Class33>(Lifestyle.Singleton);
            container.Register<IInterface34, Class34>(Lifestyle.Singleton);
            container.Register<IInterface35, Class35>(Lifestyle.Singleton);
            container.Register<IInterface36, Class36>(Lifestyle.Singleton);
            container.Register<IInterface37, Class37>(Lifestyle.Singleton);
            container.Register<IInterface38, Class38>(Lifestyle.Singleton);
            container.Register<IInterface39, Class39>(Lifestyle.Singleton);
            container.Register<IInterface40, Class40>(Lifestyle.Singleton);
            container.Register<IInterface41, Class41>(Lifestyle.Singleton);
            container.Register<IInterface42, Class42>(Lifestyle.Singleton);
            container.Register<IInterface43, Class43>(Lifestyle.Singleton);
            container.Register<IInterface44, Class44>(Lifestyle.Singleton);
            container.Register<IInterface45, Class45>(Lifestyle.Singleton);
            container.Register<IInterface46, Class46>(Lifestyle.Singleton);
            container.Register<IInterface47, Class47>(Lifestyle.Singleton);
            container.Register<IInterface48, Class48>(Lifestyle.Singleton);
            container.Register<IInterface49, Class49>(Lifestyle.Singleton);
            ;
#endregion
        }

#region resolution code
        public static void ResolveGeneric(ObjectResolver container)
        {
            var resolvedInstance0 = container.Resolve<IInterface0>();
            var resolvedInstance1 = container.Resolve<IInterface1>();
            var resolvedInstance2 = container.Resolve<IInterface2>();
            var resolvedInstance3 = container.Resolve<IInterface3>();
            var resolvedInstance4 = container.Resolve<IInterface4>();
            var resolvedInstance5 = container.Resolve<IInterface5>();
            var resolvedInstance6 = container.Resolve<IInterface6>();
            var resolvedInstance7 = container.Resolve<IInterface7>();
            var resolvedInstance8 = container.Resolve<IInterface8>();
            var resolvedInstance9 = container.Resolve<IInterface9>();
            var resolvedInstance10 = container.Resolve<IInterface10>();
            var resolvedInstance11 = container.Resolve<IInterface11>();
            var resolvedInstance12 = container.Resolve<IInterface12>();
            var resolvedInstance13 = container.Resolve<IInterface13>();
            var resolvedInstance14 = container.Resolve<IInterface14>();
            var resolvedInstance15 = container.Resolve<IInterface15>();
            var resolvedInstance16 = container.Resolve<IInterface16>();
            var resolvedInstance17 = container.Resolve<IInterface17>();
            var resolvedInstance18 = container.Resolve<IInterface18>();
            var resolvedInstance19 = container.Resolve<IInterface19>();
            var resolvedInstance20 = container.Resolve<IInterface20>();
            var resolvedInstance21 = container.Resolve<IInterface21>();
            var resolvedInstance22 = container.Resolve<IInterface22>();
            var resolvedInstance23 = container.Resolve<IInterface23>();
            var resolvedInstance24 = container.Resolve<IInterface24>();
            var resolvedInstance25 = container.Resolve<IInterface25>();
            var resolvedInstance26 = container.Resolve<IInterface26>();
            var resolvedInstance27 = container.Resolve<IInterface27>();
            var resolvedInstance28 = container.Resolve<IInterface28>();
            var resolvedInstance29 = container.Resolve<IInterface29>();
            var resolvedInstance30 = container.Resolve<IInterface30>();
            var resolvedInstance31 = container.Resolve<IInterface31>();
            var resolvedInstance32 = container.Resolve<IInterface32>();
            var resolvedInstance33 = container.Resolve<IInterface33>();
            var resolvedInstance34 = container.Resolve<IInterface34>();
            var resolvedInstance35 = container.Resolve<IInterface35>();
            var resolvedInstance36 = container.Resolve<IInterface36>();
            var resolvedInstance37 = container.Resolve<IInterface37>();
            var resolvedInstance38 = container.Resolve<IInterface38>();
            var resolvedInstance39 = container.Resolve<IInterface39>();
            var resolvedInstance40 = container.Resolve<IInterface40>();
            var resolvedInstance41 = container.Resolve<IInterface41>();
            var resolvedInstance42 = container.Resolve<IInterface42>();
            var resolvedInstance43 = container.Resolve<IInterface43>();
            var resolvedInstance44 = container.Resolve<IInterface44>();
            var resolvedInstance45 = container.Resolve<IInterface45>();
            var resolvedInstance46 = container.Resolve<IInterface46>();
            var resolvedInstance47 = container.Resolve<IInterface47>();
            var resolvedInstance48 = container.Resolve<IInterface48>();
            var resolvedInstance49 = container.Resolve<IInterface49>();
            ;
        }

        public static void ResolveNonGeneric(ObjectResolver container)
        {
            var resolvedInstance0 = container.Resolve(typeof(IInterface0));
            var resolvedInstance1 = container.Resolve(typeof(IInterface1));
            var resolvedInstance2 = container.Resolve(typeof(IInterface2));
            var resolvedInstance3 = container.Resolve(typeof(IInterface3));
            var resolvedInstance4 = container.Resolve(typeof(IInterface4));
            var resolvedInstance5 = container.Resolve(typeof(IInterface5));
            var resolvedInstance6 = container.Resolve(typeof(IInterface6));
            var resolvedInstance7 = container.Resolve(typeof(IInterface7));
            var resolvedInstance8 = container.Resolve(typeof(IInterface8));
            var resolvedInstance9 = container.Resolve(typeof(IInterface9));
            var resolvedInstance10 = container.Resolve(typeof(IInterface10));
            var resolvedInstance11 = container.Resolve(typeof(IInterface11));
            var resolvedInstance12 = container.Resolve(typeof(IInterface12));
            var resolvedInstance13 = container.Resolve(typeof(IInterface13));
            var resolvedInstance14 = container.Resolve(typeof(IInterface14));
            var resolvedInstance15 = container.Resolve(typeof(IInterface15));
            var resolvedInstance16 = container.Resolve(typeof(IInterface16));
            var resolvedInstance17 = container.Resolve(typeof(IInterface17));
            var resolvedInstance18 = container.Resolve(typeof(IInterface18));
            var resolvedInstance19 = container.Resolve(typeof(IInterface19));
            var resolvedInstance20 = container.Resolve(typeof(IInterface20));
            var resolvedInstance21 = container.Resolve(typeof(IInterface21));
            var resolvedInstance22 = container.Resolve(typeof(IInterface22));
            var resolvedInstance23 = container.Resolve(typeof(IInterface23));
            var resolvedInstance24 = container.Resolve(typeof(IInterface24));
            var resolvedInstance25 = container.Resolve(typeof(IInterface25));
            var resolvedInstance26 = container.Resolve(typeof(IInterface26));
            var resolvedInstance27 = container.Resolve(typeof(IInterface27));
            var resolvedInstance28 = container.Resolve(typeof(IInterface28));
            var resolvedInstance29 = container.Resolve(typeof(IInterface29));
            var resolvedInstance30 = container.Resolve(typeof(IInterface30));
            var resolvedInstance31 = container.Resolve(typeof(IInterface31));
            var resolvedInstance32 = container.Resolve(typeof(IInterface32));
            var resolvedInstance33 = container.Resolve(typeof(IInterface33));
            var resolvedInstance34 = container.Resolve(typeof(IInterface34));
            var resolvedInstance35 = container.Resolve(typeof(IInterface35));
            var resolvedInstance36 = container.Resolve(typeof(IInterface36));
            var resolvedInstance37 = container.Resolve(typeof(IInterface37));
            var resolvedInstance38 = container.Resolve(typeof(IInterface38));
            var resolvedInstance39 = container.Resolve(typeof(IInterface39));
            var resolvedInstance40 = container.Resolve(typeof(IInterface40));
            var resolvedInstance41 = container.Resolve(typeof(IInterface41));
            var resolvedInstance42 = container.Resolve(typeof(IInterface42));
            var resolvedInstance43 = container.Resolve(typeof(IInterface43));
            var resolvedInstance44 = container.Resolve(typeof(IInterface44));
            var resolvedInstance45 = container.Resolve(typeof(IInterface45));
            var resolvedInstance46 = container.Resolve(typeof(IInterface46));
            var resolvedInstance47 = container.Resolve(typeof(IInterface47));
            var resolvedInstance48 = container.Resolve(typeof(IInterface48));
            var resolvedInstance49 = container.Resolve(typeof(IInterface49));
            ;
        }
#endregion
    }
}