using System;
using System.Diagnostics;

namespace DpdtInject.Injector.RContext
{
    public static class ResolutionTargetHelper
    {
        public static void Dump(
            this IResolutionTarget rt
            )
        {
            IResolutionTarget? current = rt;

            Debug.WriteLine($"==================================================================");
            while (current is not null)
            {
                Debug.WriteLine($"IsRoot: {current.IsRoot}");
                Debug.WriteLine($"ClusterDeclaredType: {current.ClusterDeclaredType.FullName}");
                Debug.WriteLine($"IsGetAllResolution: {current.IsGetAllResolution}");
                Debug.WriteLine($"ScopeObject: {current.ScopeObject}");
                Debug.WriteLine($"TargetType: {current.TargetType.FullName}");

                Debug.WriteLine($"ParentRequest.ConstructorArgumentName: {current.ParentRequest.ConstructorArgumentName}");
                Debug.WriteLine($"ParentRequest.RequestedType: {current.ParentRequest.RequestedType}");
                Debug.WriteLine($"--------------------------------------------------------------");

                current = current.ParentTarget;
            }
            Debug.WriteLine($"==================================================================");
        }

        public static bool WhenAnyParentExactlyInto(
            this IResolutionTarget rt,
            Type targetType
            )
        {
            IResolutionTarget? crt = rt.ParentTarget;
            while (!(crt is null))
            {
                if (crt.TargetType == targetType)
                {
                    return true;
                }

                crt = crt.ParentTarget;
            }

            return false;
        }

        public static bool WhenAnyParentExactlyInto<T>(
            this IResolutionTarget rt
            )
        {
            return WhenAnyParentExactlyInto(rt, typeof(T));
        }

        public static bool WhenInjectedExactlyInto(
            this IResolutionTarget rt,
            Type targetType
            )
        {
            if (rt is null)
            {
                return false;
            }

            if (rt.ParentTarget is null)
            {
                return false;
            }

            if (rt.ParentTarget.TargetType != targetType)
            {
                return false;
            }

            return true;
        }

        public static bool WhenInjectedExactlyInto<T>(
            this IResolutionTarget rt
            )
        {
            return WhenInjectedExactlyInto(rt, typeof(T));
        }

        public static bool WhenInjectedExactlyNotInto<T>(
            this IResolutionTarget rt
            )
        {
            return !WhenInjectedExactlyInto(rt, typeof(T));
        }
    }
}
