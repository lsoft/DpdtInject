using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
{
    public sealed class ConstantInstanceContainer
    {
#nullable enable
        public static readonly FakeTarget? Instance;
#nullable disable

        static ConstantInstanceContainer()
        {
            //GENERATOR: init constant
        }


        private ConstantInstanceContainer()
        {
        }

#if UNDECLARED_SYMBOL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPredicate(ResolutionContext resolutionContext)
        {
            Func<IResolutionContext, bool> predicate = //GENERATOR: predicate
            ;

            var result = predicate(resolutionContext);
            return result;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FakeTarget GetInstance(ResolutionContext resolutionContext)
        {
            return Instance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<FakeTarget> GetInstance_Func(ResolutionContext resolutionContext)
        {
            return () => Instance;
        }

    }

}
