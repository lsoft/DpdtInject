//#nullable disable
using System;
using System.Collections.Generic;
using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.RContext;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Binding.Example.UnConditional
{
    //#nullable enable
    public partial class DpdtModule0 : DpdtModule
    {
        private readonly Provider _provider = new Provider();
        public override void Dispose()
        {
            IA_A_SingletonInstanceContainer_3c8dfbff_bf62_4d86_9fab_43ecb1d36529.DoDisposeIfApplicable();
            IB_B_SingletonInstanceContainer_26ae20ef_3980_41b5_9497_c37cf42bc440.DoDisposeIfApplicable();
            IC_C_SingletonInstanceContainer_dd4afbe7_ace9_4ed6_8f9e_6c1bce17a03c.DoDisposeIfApplicable();
        }

        public T Get<T>()
        {
            return ((IBaseProvider<T>)_provider).Get();
        }

        public List<T> GetAll<T>()
        {
            return ((IBaseProvider<T>)_provider).GetAll();
        }

        private class Provider : IBaseProvider<IA>, IBaseProvider<IB>, IBaseProvider<IC>
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IA IBaseProvider<IA>.Get()
            {
                return IA_A_SingletonInstanceContainer_3c8dfbff_bf62_4d86_9fab_43ecb1d36529.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IA> IBaseProvider<IA>.GetAll()
            {
                var result = new List<IA>();
                result.Add(IA_A_SingletonInstanceContainer_3c8dfbff_bf62_4d86_9fab_43ecb1d36529.GetInstance(null));
                return result;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IB IBaseProvider<IB>.Get()
            {
                return IB_B_SingletonInstanceContainer_26ae20ef_3980_41b5_9497_c37cf42bc440.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IB> IBaseProvider<IB>.GetAll()
            {
                var result = new List<IB>();
                result.Add(IB_B_SingletonInstanceContainer_26ae20ef_3980_41b5_9497_c37cf42bc440.GetInstance(null));
                return result;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IC IBaseProvider<IC>.Get()
            {
                return IC_C_SingletonInstanceContainer_dd4afbe7_ace9_4ed6_8f9e_6c1bce17a03c.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IC> IBaseProvider<IC>.GetAll()
            {
                var result = new List<IC>();
                result.Add(IC_C_SingletonInstanceContainer_dd4afbe7_ace9_4ed6_8f9e_6c1bce17a03c.GetInstance(null));
                return result;
            }
        }

//#nullable disable
        private sealed class IA_A_SingletonInstanceContainer_3c8dfbff_bf62_4d86_9fab_43ecb1d36529
        {
//#nullable enable
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction = () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            }

            ;
            private IA_A_SingletonInstanceContainer_3c8dfbff_bf62_4d86_9fab_43ecb1d36529()
            {
            }

#if UNDECLARED_SYMBOL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPredicate(ResolutionContext resolutionContext)
        {
            Func<IResolutionContext, bool> predicate = rc => true
            ;

            var result = predicate(resolutionContext);
            return result;
        }
#endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static A GetInstance(ResolutionContext resolutionContext)
            {
                Interlocked.Exchange(ref _currentDisposeAction, _realDisposeAction);
                return Nested.GetInstance(resolutionContext);
            }

            public static void DoDisposeIfApplicable()
            {
                Interlocked.Exchange(ref _currentDisposeAction, null)?.Invoke();
            }

            private class Nested
            {
                private static readonly object _locker = new object();
                public static volatile A Instance = null;
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static A GetInstance(ResolutionContext resolutionContext)
                {
                    if (Instance is null)
                    {
                        lock (_locker)
                        {
                            if (Instance is null)
                            {
                                Instance = new A();
                            }
                        }
                    }

                    return Instance;
                }
            }
//#nullable disable
        }

        private sealed class IB_B_SingletonInstanceContainer_26ae20ef_3980_41b5_9497_c37cf42bc440
        {
//#nullable enable
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction = () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            }

            ;
            private IB_B_SingletonInstanceContainer_26ae20ef_3980_41b5_9497_c37cf42bc440()
            {
            }

#if UNDECLARED_SYMBOL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPredicate(ResolutionContext resolutionContext)
        {
            Func<IResolutionContext, bool> predicate = rc => true
            ;

            var result = predicate(resolutionContext);
            return result;
        }
#endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static B GetInstance(ResolutionContext resolutionContext)
            {
                Interlocked.Exchange(ref _currentDisposeAction, _realDisposeAction);
                return Nested.GetInstance(resolutionContext);
            }

            public static void DoDisposeIfApplicable()
            {
                Interlocked.Exchange(ref _currentDisposeAction, null)?.Invoke();
            }

            private class Nested
            {
                private static readonly object _locker = new object();
                public static volatile B Instance = null;
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static IA Get_a(ResolutionContext resolutionContext)
                {
                    return IA_A_SingletonInstanceContainer_3c8dfbff_bf62_4d86_9fab_43ecb1d36529.GetInstance(null);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static B GetInstance(ResolutionContext resolutionContext)
                {
                    if (Instance is null)
                    {
                        lock (_locker)
                        {
                            if (Instance is null)
                            {
                                Instance = new B(message: Message, a: Get_a(resolutionContext));
                            }
                        }
                    }

                    return Instance;
                }
            }
//#nullable disable
        }

        private sealed class IC_C_SingletonInstanceContainer_dd4afbe7_ace9_4ed6_8f9e_6c1bce17a03c
        {
//#nullable enable
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction = () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            }

            ;
            private IC_C_SingletonInstanceContainer_dd4afbe7_ace9_4ed6_8f9e_6c1bce17a03c()
            {
            }

#if UNDECLARED_SYMBOL
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPredicate(ResolutionContext resolutionContext)
        {
            Func<IResolutionContext, bool> predicate = rc => true
            ;

            var result = predicate(resolutionContext);
            return result;
        }
#endif
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static C GetInstance(ResolutionContext resolutionContext)
            {
                Interlocked.Exchange(ref _currentDisposeAction, _realDisposeAction);
                return Nested.GetInstance(resolutionContext);
            }

            public static void DoDisposeIfApplicable()
            {
                Interlocked.Exchange(ref _currentDisposeAction, null)?.Invoke();
            }

            private class Nested
            {
                private static readonly object _locker = new object();
                public static volatile C Instance = null;
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static IB Get_b(ResolutionContext resolutionContext)
                {
                    return IB_B_SingletonInstanceContainer_26ae20ef_3980_41b5_9497_c37cf42bc440.GetInstance(null);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static C GetInstance(ResolutionContext resolutionContext)
                {
                    if (Instance is null)
                    {
                        lock (_locker)
                        {
                            if (Instance is null)
                            {
                                Instance = new C(b: Get_b(resolutionContext));
                            }
                        }
                    }

                    return Instance;
                }
            }
//#nullable disable
        }
    }
}