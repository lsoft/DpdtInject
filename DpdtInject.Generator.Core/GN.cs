using DpdtInject.Generator.Core.Producer;
using DpdtInject.Injector;
using DpdtInject.Injector.RContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Generator.Core
{
    public static class GN
    {
        public static string AggressiveInlining = "[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]";

        public static string IResolutionTarget = typeof(IResolutionTarget).ToGlobalDisplayString();
        public static string ResolutionTarget = "global::" + typeof(ResolutionTarget<,>).Namespace + "." + nameof(ResolutionTarget<object, object>);

        public static string IResolutionRequest = typeof(IResolutionRequest).ToGlobalDisplayString();
        public static string ResolutionRequest = "global::" + typeof(ResolutionRequest<>).Namespace + "." + nameof(ResolutionRequest<object, object>);

        public static string IResolution = "global::" + typeof(IResolution<>).Namespace + "." + nameof(IResolution<object>);
        public static string IResolutionFast = "global::" + typeof(IResolutionFast<>).Namespace + "." + nameof(IResolutionFast<object>);

        public static string IDisposable = typeof(IDisposable).ToGlobalDisplayString();
    }
}
