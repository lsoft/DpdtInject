namespace DpdtInject.Injector.Module.Bind
{
    //public static class ConditionalHelper
    //{
    //    public static IConfigureBinding WhenInjectedExactlyInto<T>(
    //        this IConfigureAndConditionalBinding c
    //        )
    //    {
    //        return WhenInjectedExactlyInto(c, typeof(T));
    //    }

    //    public static IConfigureBinding WhenInjectedExactlyInto(
    //        this IConfigureAndConditionalBinding c,
    //        Type t
    //        )
    //    {
    //        if (t is null)
    //        {
    //            throw new ArgumentNullException(nameof(t));
    //        }

    //        return c.When(
    //            DpdtIdempotentStatusEnum.Idempotent,
    //            cc => cc.ParentContext != null
    //                && cc.ParentContext.InstanceContainerInfo.Configuration.BindNode.BindTo == t
    //            );
    //    }
    //}
}
