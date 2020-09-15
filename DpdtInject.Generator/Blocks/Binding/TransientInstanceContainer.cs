using DpdtInject.Injector.Module.RContext;
using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace DpdtInject.Generator.Blocks.Binding
{
    public static class TransientInstanceContainer
    {
#nullable enable

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CheckPredicate(ResolutionContext resolutionContext)
        {
            return DateTime.Now.Millisecond % 2 == 0;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FakeTarget GetInstance()
        {
            var result = new FakeTarget(
                //GENERATOR: arguments
                );

            return result;
        }
#nullable disable
    }

}
