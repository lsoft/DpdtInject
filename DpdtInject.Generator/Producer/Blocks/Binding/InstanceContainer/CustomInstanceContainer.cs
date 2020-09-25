using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
{
    public sealed class CustomInstanceContainer
    {
        private CustomInstanceContainer()
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Func<FakeTarget> GetInstance_Func(ResolutionContext resolutionContext)
        {
            return Nested.GetInstance_Func(resolutionContext);
        }


#nullable enable
        private class Nested
        {
            private static readonly Guid CustomScopeUniqueId = new Guid(" /*GENERATOR: put here unique guid*/ ");

            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested()
            {
            }

            //GENERATOR: argument methods

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static FakeTarget GetInstance(ResolutionContext resolutionContext)
            {

                var instance = (FakeTarget)resolutionContext.ScopeObject!.GetOrAdd(
                    CustomScopeUniqueId,
                    () => new FakeTarget(
                    //GENERATOR: apply arguments
                    )
                );

                return instance;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static Func<FakeTarget> GetInstance_Func(ResolutionContext resolutionContext)
            {
                Func<FakeTarget> instance = () => GetInstance(
                    resolutionContext
                    );

                return instance;
            }

        }
#nullable disable
    }

}
