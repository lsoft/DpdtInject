using DpdtInject.Injector;
using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace DpdtInject.Tests.Performance.NonGeneric.PreProgrammed
{
    public partial class DpdtPerformanceModule //: DpdtModule
    {
        private readonly Provider _provider = new Provider();
        //public override void Dispose()
        //{
        //}

        public T Get<T>()
        {
            return ((IBaseProvider<T>)_provider).Get();
        }

        public List<T> GetAll<T>()
        {
            return ((IBaseProvider<T>)_provider).GetAll();
        }

        public object Get(System.Type requestedType)
        {
            return _provider.Get(requestedType);
        }

        public List<object> GetAll(System.Type requestedType)
        {
            return _provider.GetAll(requestedType);
        }

        private class Provider : IBaseProvider<IA>, IBaseProvider<IB>, IBaseProvider<IC>
        {
            private readonly DpdtInject.Injector.ReinventedContainer _typeContainer;
            public Provider()
            {
                _typeContainer = new DpdtInject.Injector.ReinventedContainer(
                    (typeof(IA), new Func<object>(() => ((IBaseProvider<IA>)this).Get())),
                    (typeof(IB), new Func<object>(() => ((IBaseProvider<IB>)this).Get())),
                    (typeof(IC), new Func<object>(() => ((IBaseProvider<IC>)this).Get()))
                    );
            }

            public object Get(System.Type requestedType)
            {
                var resolveFunc = _typeContainer.GetGet(requestedType);
                if (resolveFunc is null)
                {
                    throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, string.Format("No bindings available for {0}", requestedType.FullName), requestedType.FullName);
                }

                return resolveFunc();
            }

            public List<object> GetAll(System.Type requestedType)
            {
                var result = new List<object>();
                var resolveTuples = _typeContainer.GetGetAllDirty(requestedType);
                if (resolveTuples is null)
                {
                    throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, string.Format("No bindings available for {0}", requestedType.FullName), requestedType.FullName);
                }

                for (var index = 0; index < resolveTuples.Count; index++)
                {
                    var tuple = resolveTuples[index];
                    if (tuple.Type != requestedType)
                    {
                        continue;
                    }

                    result.Add(tuple.Factory());
                }

                //ReinventedContainer can return null or list of completely unsuitable items
                //because of its hashing nature
                //so we need to do additional check in that case
                if (result.Count == 0)
                {
                    if (!_typeContainer.IsTypeKnown(requestedType))
                    {
                        throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, string.Format("No bindings available for {0}", requestedType.FullName), requestedType.FullName);
                    }
                }

                return result;
            }

            private static readonly ResolutionContext Context_DpdtInject_Tests_Transient_Hierarchy3_IA = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IA)));
            private static readonly ResolutionContext Context_DpdtInject_Tests_Transient_Hierarchy3_IB = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IB)));
            private static readonly ResolutionContext Context_DpdtInject_Tests_Transient_Hierarchy3_IC = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IC)));
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IA IBaseProvider<IA>.Get()
            {
                return IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IA> IBaseProvider<IA>.GetAll()
            {
                var result = new List<IA>();
                result.Add(IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1.GetInstance(null));
                return result;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IB IBaseProvider<IB>.Get()
            {
                return IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IB> IBaseProvider<IB>.GetAll()
            {
                var result = new List<IB>();
                result.Add(IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7.GetInstance(null));
                return result;
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IC IBaseProvider<IC>.Get()
            {
                return IC_C_TransientInstanceContainer_653d7d82_3d4a_44f3_a57a_c4ad03a45056.GetInstance(null);
            }

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IC> IBaseProvider<IC>.GetAll()
            {
                var result = new List<IC>();
                result.Add(IC_C_TransientInstanceContainer_653d7d82_3d4a_44f3_a57a_c4ad03a45056.GetInstance(null));
                return result;
            }
        }

#nullable disable
        private sealed class IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1
        {
#nullable enable
            private IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1()
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
                return Nested.GetInstance(resolutionContext);
            }

            private class Nested
            {
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static A GetInstance(ResolutionContext resolutionContext)
                {
                    var instance = new A();
                    return instance;
                }
            }
#nullable disable
        }

        private sealed class IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7
        {
#nullable enable
            private IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7()
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
                return Nested.GetInstance(resolutionContext);
            }

            private class Nested
            {
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static IA Get_a(ResolutionContext resolutionContext)
                {
                    return IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1.GetInstance(null);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static B GetInstance(ResolutionContext resolutionContext)
                {
                    var instance = new B(a: Get_a(resolutionContext));
                    return instance;
                }
            }
#nullable disable
        }

        private sealed class IC_C_TransientInstanceContainer_653d7d82_3d4a_44f3_a57a_c4ad03a45056
        {
#nullable enable
            private IC_C_TransientInstanceContainer_653d7d82_3d4a_44f3_a57a_c4ad03a45056()
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
                return Nested.GetInstance(resolutionContext);
            }

            private class Nested
            {
                // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
                static Nested()
                {
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                private static IB Get_b(ResolutionContext resolutionContext)
                {
                    return IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7.GetInstance(null);
                }

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal static C GetInstance(ResolutionContext resolutionContext)
                {
                    var instance = new C(b: Get_b(resolutionContext));
                    return instance;
                }
            }
#nullable disable
        }
    }
}
