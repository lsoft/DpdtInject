using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
{
    public sealed class SingletonInstanceContainer
    {
        private SingletonInstanceContainer()
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

        public static void DoDisposeIfApplicable()
        {
            if (Nested.Instance is IDisposable disposableInstance)
            {
                disposableInstance.Dispose();
            }
        }

#nullable enable
        private class Nested
        {
            private static readonly object _locker = new object();
            public static volatile FakeTarget? Instance = null;

            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested()
            {
            }

            //GENERATOR: declare arguments

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal static FakeTarget GetInstance(ResolutionContext resolutionContext)
            {
                if(Instance is null)
                {
                    lock(_locker)
                    {
                        if(Instance is null)
                        {
                            Instance = new FakeTarget(
                                //GENERATOR: apply arguments
                              );
                        }
                    }
                }

                return Instance!;
            }
        }
#nullable disable
    }

}
