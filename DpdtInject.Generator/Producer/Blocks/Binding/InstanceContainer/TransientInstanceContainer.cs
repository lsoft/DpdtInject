using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
{
    public sealed class TransientInstanceContainer
    {
        private TransientInstanceContainer()
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
            return Nested.GetInstance(resolutionContext);
        }

#nullable enable
        private class Nested
        {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested()
            {
            }

            //GENERATOR: declare arguments

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static FakeTarget GetInstance(ResolutionContext resolutionContext)
            {
                var instance = new FakeTarget(
                    //GENERATOR: apply arguments
                    );

                return instance;
            }
        }
#nullable disable
    }

}
