//#nullabledisable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.Bind;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DpdtInject.Injector.Module.RContext;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DpdtInject.Injector.Module.RContext;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DpdtInject.Injector.Module.RContext;

namespace DpdtInject.Binding.Example.Conditional
{
//#nullableenable
    public partial class DpdtModule0 : DpdtModule
    {
        private readonly Provider _provider = new Provider();
        public override void Dispose()
        {
            IA_A_SingletonInstanceContainer_6a500c80_6530_4fd1_9315_72f9b1f5bef5.DoDisposeIfApplicable();
            IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.DoDisposeIfApplicable();
            IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451.DoDisposeIfApplicable();
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
                return IA_A_SingletonInstanceContainer_6a500c80_6530_4fd1_9315_72f9b1f5bef5.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IA> IBaseProvider<IA>.GetAll()
            {
                var result = new List<IA>();
                result.Add(IA_A_SingletonInstanceContainer_6a500c80_6530_4fd1_9315_72f9b1f5bef5.GetInstance(null));
                return result;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IB IBaseProvider<IB>.Get()
            {
                var context = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IB)));
                if (IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.CheckPredicate(context))
                {
                    return IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.GetInstance(context);
                }

                throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, "No bindings available for [IB]", "IB");
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IB> IBaseProvider<IB>.GetAll()
            {
                var result = new List<IB>();
                var context = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IB)));
                if (IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.CheckPredicate(context))
                {
                    result.Add(IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.GetInstance(context));
                }

                return result;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IC IBaseProvider<IC>.Get()
            {
                var context = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IC)));
                if (IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451.CheckPredicate(context))
                {
                    return IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451.GetInstance(context);
                }

                throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, "No bindings available for [IC]", "IC");
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IC> IBaseProvider<IC>.GetAll()
            {
                var result = new List<IC>();
                var context = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IC)));
                if (IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451.CheckPredicate(context))
                {
                    result.Add(IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451.GetInstance(context));
                }

                return result;
            }
        }

//#nullabledisable
        private sealed class IA_A_SingletonInstanceContainer_6a500c80_6530_4fd1_9315_72f9b1f5bef5
        {
//#nullableenable
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction = () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            }

            ;
            private IA_A_SingletonInstanceContainer_6a500c80_6530_4fd1_9315_72f9b1f5bef5()
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
                private static readonly object _locker = new object ();
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
//#nullabledisable
        }

        private sealed class IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87
        {
//#nullableenable
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction = () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            }

            ;
            private IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87()
            {
            }

#if !UNDECLARED_SYMBOL
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool CheckPredicate(ResolutionContext resolutionContext)
            {
                Func<IResolutionContext, bool> predicate = rc => true;
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
                private static readonly object _locker = new object ();
                public static volatile B Instance = null;
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static IA Get_a(ResolutionContext resolutionContext)
                {
                    return IA_A_SingletonInstanceContainer_6a500c80_6530_4fd1_9315_72f9b1f5bef5.GetInstance(null);
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
//#nullabledisable
        }

        private sealed class IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451
        {
//#nullableenable
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction = () =>
            {
                if (Nested.Instance is IDisposable disposableInstance)
                {
                    disposableInstance.Dispose();
                }
            }

            ;
            private IC_C_SingletonInstanceContainer_32560e92_6d98_4f46_b9fc_26c25f66b451()
            {
            }

#if !UNDECLARED_SYMBOL
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool CheckPredicate(ResolutionContext resolutionContext)
            {
                Func<IResolutionContext, bool> predicate = rc => true;
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
                private static readonly object _locker = new object ();
                public static volatile C Instance = null;
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static IB Get_b(ResolutionContext resolutionContext)
                {
                    var context = resolutionContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IB), "b"));
                    if (IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.CheckPredicate(context))
                    {
                        return IB_B_SingletonInstanceContainer_5d75f850_504c_4635_aa2f_ef31fb526e87.GetInstance(context);
                    }

                    throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, "No bindings [IB] available for [C]", "C");
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
//#nullabledisable
        }
    }
}