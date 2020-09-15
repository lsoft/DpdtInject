using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
{
    public sealed class SingletonInstanceContainer
    {
#nullable enable
        private volatile static Action? _currentDisposeAction = null;
        private volatile static Action _realDisposeAction =
            () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            };

        private SingletonInstanceContainer()
        {
        }

#if UNDECLARED_SYMBOL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPredicate(ResolutionContext resolutionContext)
        {
            Func<ResolutionContext, bool> predicate = //GENERATOR: predicate
            ;

            var result = predicate(resolutionContext);
            return result;
        }
#endif

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FakeTarget GetInstance()
        {
            Interlocked.Exchange(ref _currentDisposeAction, _realDisposeAction);

            return Nested.Instance;
        }

        public static void DoDisposeIfApplicable()
        {
            Interlocked.Exchange(ref _currentDisposeAction, null)?.Invoke();
        }

        private class Nested
        {
            // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
            static Nested()
            {
            }

            internal static readonly FakeTarget Instance = new FakeTarget(
                //GENERATOR: arguments
                );
        }
#nullable disable
    }

}
