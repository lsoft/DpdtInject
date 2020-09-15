using System;
using System.Runtime.CompilerServices;
using System.Threading;
using DpdtInject.Binding.Example.Zero;

namespace DpdtInject.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var module = new D2pdt2Module0();

            var a1 = module.Get<IA1>();
            var a2 = module.Get<IA2>();
            var b = module.Get<IB>();
            var c = module.Get<IC>();

            var aall = module.GetAll<IA1>();

            //var kernelBuilder = new D2pdt2KernelBuilder();
            //var kernel0 = kernelBuilder.GetKernel<D2pdt2Kernel0>();
            //kernel0.ShowMessage();


            //var kernel0 = new D2pdt2Kernel0();
            //kernel0.Load(new D2pdt2Module0());

            ///D_Singleton_Container.CheckPredicate();
            //D_Singleton_Container.GetInstance();
            //D_Singleton_Container.DoDisposeIfApplicable();

            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("OK");
            //Console.ResetColor();
        }
    }


    //public sealed class D_Singleton_Container
    //{
    //    private volatile static Action _currentDisposeAction = null;
    //    private volatile static Action _realDisposeAction =
    //        () =>
    //        {
    //            if (Nested.Instance is IDisposable disposableInstance)
    //            {
    //                disposableInstance.Dispose();
    //            }
    //        };

    //    private D_Singleton_Container()
    //    {
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static bool CheckPredicate()
    //    {
    //        return DateTime.Now.Millisecond % 2 == 0;
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    public static D GetInstance()
    //    {
    //        Interlocked.Exchange(ref _currentDisposeAction, _realDisposeAction);

    //        return Nested.Instance;
    //    }

    //    public static void DoDisposeIfApplicable()
    //    {
    //        Interlocked.Exchange(ref _currentDisposeAction, null)?.Invoke();
    //    }

    //    private class Nested
    //    {
    //        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
    //        static Nested()
    //        {
    //        }

    //        internal static readonly D Instance = new D(
                
    //            );
    //    }
    //}


    //public class D : IDisposable
    //{
    //    public D()
    //    {
    //        Console.WriteLine("D created");

    //    }

    //    public void Dispose()
    //    {
    //        Console.WriteLine("D disposed");
    //    }
    //}


}

