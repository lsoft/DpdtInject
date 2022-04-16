using DpdtInject.Generator.Core.Producer;
using System;
using DpdtInject.Injector.Src.RContext;
using DpdtInject.Injector.Src;

namespace DpdtInject.Generator.Core
{
    public static class GN
    {
        public static string IResolutionTarget = typeof(IResolutionTarget).ToGlobalDisplayString();
        public static string ResolutionTarget = "global::" + typeof(ResolutionTarget<,>).Namespace + "." + nameof(ResolutionTarget<object, object>);

        public static string IResolutionRequest = typeof(IResolutionRequest).ToGlobalDisplayString();
        public static string ResolutionRequest = "global::" + typeof(ResolutionRequest<>).Namespace + "." + nameof(ResolutionRequest<object, object>);

        public static string IResolution = "global::" + typeof(IResolution<>).Namespace + "." + nameof(IResolution<object>);
        public static string IResolutionFast = "global::" + typeof(IResolutionFast<>).Namespace + "." + nameof(IResolutionFast<object>);

        public static string IDisposable = typeof(IDisposable).ToGlobalDisplayString();
        
        public static string CustomScopeObject = "global::" + typeof(DpdtInject.Injector.Src.CustomScope.CustomScopeObject).Namespace + "." + nameof(DpdtInject.Injector.Src.CustomScope.CustomScopeObject);
        public static string CustomScopeObject_None = "global::" + typeof(DpdtInject.Injector.Src.CustomScope.CustomScopeObject).Namespace + "." + nameof(DpdtInject.Injector.Src.CustomScope.CustomScopeObject) + "." + nameof(DpdtInject.Injector.Src.CustomScope.CustomScopeObject.None);

        public static string Exception = "global::" + typeof(System.Exception).Namespace + "." + nameof(System.Exception);

        //we can't define it as above, because there is no AggressiveInlining, ValueTask, IAsyncDisposable in netstandard2.0,
        //but source generators are compiled in netstandard2.0
        //so, declare it by the simplest way:
        public static string AggressiveInlining = "[global::System.Runtime.CompilerServices.MethodImpl(global::System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]";
        public static string IAsyncDisposable = "global::System.IAsyncDisposable";
        public static string ValueTask = "global::System.Threading.Tasks.ValueTask";
    }
}
