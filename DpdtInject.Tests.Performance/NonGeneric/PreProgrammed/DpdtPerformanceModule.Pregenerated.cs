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
        private static readonly Provider _provider;
        private static readonly DpdtInject.Injector.ReinventedContainer _typeContainer;

        //public override void Dispose()
        //{
        //}

        //Func<object> resolveFuncIA, resolveFuncIB, resolveFuncIC;


        static DpdtPerformanceModule()
        {
            _provider = new Provider();

            _typeContainer = new DpdtInject.Injector.ReinventedContainer(
                //(typeof(IA), new Func<object>(() => ((IBaseProvider<IA>)_provider).Get())),
                //(typeof(IB), new Func<object>(() => ((IBaseProvider<IB>)_provider).Get())),
                //(typeof(IC), new Func<object>(() => ((IBaseProvider<IC>)_provider).Get()))
                new Tuple<Type, Func<object>>(typeof(IA), _provider.Get_IA),
                new Tuple<Type, Func<object>>(typeof(IB), _provider.Get_IB),
                new Tuple<Type, Func<object>>(typeof(IC), _provider.Get_IC)
                );

            ////resolveFuncIA = () => ((IBaseProvider<IA>)_provider).Get();// TypeContainer.GetGet(requestedType);
            ////resolveFuncIB = () => ((IBaseProvider<IB>)_provider).Get();// TypeContainer.GetGet(requestedType);
            ////resolveFuncIC = () => ((IBaseProvider<IC>)_provider).Get();// TypeContainer.GetGet(requestedType);
            //resolveFuncIA = () => _provider.Get_IA();// TypeContainer.GetGet(requestedType);
            //resolveFuncIB = () => _provider.Get_IB();// TypeContainer.GetGet(requestedType);
            //resolveFuncIC = () => _provider.Get_IC();// TypeContainer.GetGet(requestedType);
        }

        public T Get<T>()
        {
            return ((IBaseProvider<T>)_provider).Get();
        }

        public List<T> GetAll<T>()
        {
            return ((IBaseProvider<T>)_provider).GetAll();
        }

        //public object GetIA(System.Type requestedType)
        //{
        //    return _provider.Get_IA();//resolveFuncIA();
        //}
        //public object GetIB(System.Type requestedType)
        //{
        //    return _provider.Get_IB(); //resolveFuncIB();
        //}

        //public object GetIC(System.Type requestedType)
        //{
        //    return _provider.Get_IC(); //resolveFuncIC();
        //}

        public object Get(System.Type requestedType)
        {
            return _typeContainer.GetGetObject(requestedType);
        }

        public List<object> GetAll(System.Type requestedType)
        {
            throw new NotImplementedException();

            //var result = new List<object>();
            //var resolveTuples = _typeContainer.GetGetAllDirty(requestedType);
            //if (resolveTuples is null)
            //{
            //    throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, string.Format("No bindings available for {0}", requestedType.FullName), requestedType.FullName);
            //}

            //for (var index = 0; index < resolveTuples.Count; index++)
            //{
            //    var tuple = resolveTuples[index];
            //    if (tuple.Type != requestedType)
            //    {
            //        continue;
            //    }

            //    result.Add(tuple.Factory());
            //}

            ////ReinventedContainer can return null or list of completely unsuitable items
            ////because of its hashing nature
            ////so we need to do additional check in that case
            //if (result.Count == 0)
            //{
            //    if (!_typeContainer.IsTypeKnown(requestedType))
            //    {
            //        throw new DpdtInject.Injector.Excp.DpdtException(DpdtExceptionTypeEnum.NoBindingAvailable, string.Format("No bindings available for {0}", requestedType.FullName), requestedType.FullName);
            //    }
            //}

            //return result;
        }

        private class Provider : IBaseProvider<IA>, IBaseProvider<IB>, IBaseProvider<IC>
        {
            public Provider()
            {
            }


            private static readonly ResolutionContext Context_DpdtInject_Tests_Transient_Hierarchy3_IA = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IA)));
            private static readonly ResolutionContext Context_DpdtInject_Tests_Transient_Hierarchy3_IB = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IB)));
            private static readonly ResolutionContext Context_DpdtInject_Tests_Transient_Hierarchy3_IC = DpdtInject.Injector.Module.RContext.ResolutionContext.EmptyContext.AddFrame(new DpdtInject.Injector.Module.RContext.ResolutionFrame(typeof(IC)));

            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Get_IA()
            {
                return IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1.GetInstance(null);
            }
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Get_IB()
            {
                return IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7.GetInstance(null);
            }
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            public object Get_IC()
            {
                return IC_C_TransientInstanceContainer_653d7d82_3d4a_44f3_a57a_c4ad03a45056.GetInstance(null);

            }


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
                var instance = new A();
                return instance;
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
            private static IA Get_a(ResolutionContext resolutionContext)
            {
                return IA_A_TransientInstanceContainer_49f8f06d_6490_45d6_82e4_df59eefea2d1.GetInstance(null);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static B GetInstance(ResolutionContext resolutionContext)
            {
                var instance = new B(a: Get_a(resolutionContext));
                return instance;
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
            private static IB Get_b(ResolutionContext resolutionContext)
            {
                return IB_B_TransientInstanceContainer_97ea037b_4986_484f_9e45_b205bd097ea7.GetInstance(null);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static C GetInstance(ResolutionContext resolutionContext)
            {
                var instance = new C(b: Get_b(resolutionContext));
                return instance;
            }

#nullable disable
        }
    }
}
