using DpdtInject.Injector;
using DpdtInject.Injector.Module;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Binding.Example.One
{
    //public partial class D2pdt2Kernel1 : D2pdt2Kernel
    //{

    //}

    public partial class D2pdt2Module1 : DpdtModule
    {

        private readonly Provider _provider = new Provider();

        public override void Dispose()
        {
            IA_A1_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5.DoDisposeIfApplicable();
            IA_A2_Singleton_Container_62EE618A_15CB_481F_9FA8_2E12BDDD8544.DoDisposeIfApplicable();
        }

        public T Get<T>()
        {
            return ((IBaseProvider<T>)_provider).Get();
        }


        public List<T> GetAll<T>()
        {
            return ((IBaseProvider<T>)_provider).GetAll();
        }





        private class Provider : 
            IBaseProvider<IA>
        {
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            IA IBaseProvider<IA>.Get()
            {
                throw new InvalidOperationException("слишком много биндингов IA, неясно какой отдавать");
            }
            //[MethodImpl(MethodImplOptions.AggressiveInlining)]
            List<IA> IBaseProvider<IA>.GetAll()
            {
                return new List<IA>
                {
                    IA_A1_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5.GetInstance(),
                    IA_A2_Singleton_Container_62EE618A_15CB_481F_9FA8_2E12BDDD8544.GetInstance(),
                };
            }
        }




        public sealed class IA_A2_Singleton_Container_62EE618A_15CB_481F_9FA8_2E12BDDD8544
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

            private IA_A2_Singleton_Container_62EE618A_15CB_481F_9FA8_2E12BDDD8544()
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static A2 GetInstance()
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

                internal static readonly A2 Instance = new A2(
                    //SomeString
                    );
            }
        }

        public sealed class IA_A1_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5
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

            private IA_A1_Singleton_Container_AC5CC60D_A385_4A7B_A741_4089E650CEB5()
            {
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static A1 GetInstance()
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

                internal static readonly A1 Instance = new A1(
                    SomeString
                    );
            }
        }


    }


}

