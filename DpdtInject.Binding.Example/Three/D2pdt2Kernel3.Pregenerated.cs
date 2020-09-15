using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Binding.Example.Three
{
    //public partial class D2pdt2Kernel0 : D2pdt2Kernel
    //{

    //}

    public partial class D2pdt2Module3 : DpdtModule
    {
        public const string SomeString = "hello guys!";

        private readonly Provider _provider = new Provider();

        public override void Dispose()
        {
            IB_B_Singleton_Container_E0C46AC9_14EA_4142_A7D5_17460AA7AB90.DoDisposeIfApplicable();
            IA_A_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5.DoDisposeIfApplicable();
        }

        public T Get<T>()
        {
            return ((IBaseProvider<T>)_provider).Get();
        }


        //public List<T> GetAll<T>()
        //{
        //    return ((IBaseProvider<T>)_provider).GetAll();
        //}

        public interface IBaseProvider<T>
        {
            T Get();

            //List<T> GetAll();
        }


        private class Provider :
            IBaseProvider<IA>,
            IBaseProvider<IB>
        {


            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IA IBaseProvider<IA>.Get()
            {
                return IA_A_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5.GetInstance();
            }
            ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
            //List<IA> IBaseProvider<IA>.GetAll()
            //{
            //    return
            //        new List<IA>
            //        {
            //            IA1_IA2_A_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5.GetInstance()
            //        };
            //}


            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IB IBaseProvider<IB>.Get()
            {
                return IB_B_Singleton_Container_E0C46AC9_14EA_4142_A7D5_17460AA7AB90.GetInstance();
            }
            ////[MethodImpl(MethodImplOptions.AggressiveInlining)]
            //List<IB> IBaseProvider<IB>.GetAll()
            //{
            //    return
            //        new List<IB>
            //        {
            //            IB_B_Singleton_Container_E0C46AC9_14EA_4142_A7D5_17460AA7AB90.GetInstance()
            //        };
            //}
        }


        public class ResolutionFrame
        {
        }


        public class ResolutionContext
        {
            public static readonly ResolutionContext EmptyContext = new ResolutionContext();

            public IReadOnlyList<ResolutionFrame> Frames
            {
                get;
            }

            public ResolutionContext AddFrame(ResolutionFrame newFrame)
            {
                throw new NotImplementedException();
            }
        }


        public sealed class IB_B_Singleton_Container_E0C46AC9_14EA_4142_A7D5_17460AA7AB90
        {
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction =
                () =>
                {
                    if (Nested.Instance is IDisposable disposableInstance)
                    {
                        disposableInstance.Dispose();
                    }
                };

            private IB_B_Singleton_Container_E0C46AC9_14EA_4142_A7D5_17460AA7AB90()
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool CheckPredicate(ResolutionContext resolutionContext)
            {
                return DateTime.Now.Millisecond % 2 == 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static B GetInstance()
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

                internal static readonly B Instance = new B(
                    IA_A_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5.GetInstance()
                    );
            }
        }

        public sealed class IA_A_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5
        {
            private volatile static Action _currentDisposeAction = null;
            private volatile static Action _realDisposeAction =
                () =>
                {
                    if (Nested.Instance is IDisposable disposableInstance)
                    {
                        disposableInstance.Dispose();
                    }
                };

            private IA_A_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5()
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool CheckPredicate(ResolutionContext resolutionContext)
            {
                return DateTime.Now.Millisecond % 2 == 0;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static A GetInstance()
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

                internal static readonly A Instance = new A(
                    //SomeString
                    );
            }
        }


    }


}

