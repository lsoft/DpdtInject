using System;

namespace DpdtInject.Injector.Module.RContext
{
    public static class ResolutionTargetHelper
    {
        public static bool WhenAnyParentExactlyInto(
            this IResolutionTarget rt,
            Type targetType
            )
        {
            while(!(rt is null))
            {
                if (rt.TargetType == targetType)
                {
                    return true;
                }

                rt = rt.ParentTarget;
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

            if (rt.TargetType != targetType)
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
    }
}
