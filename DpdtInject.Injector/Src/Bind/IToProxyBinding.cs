using System;

namespace DpdtInject.Injector.Bind
{
    public interface IToProxyBinding
    {
        IScopeBinding WithProxySettings<TAttribute, TSessionSaver>()
            where TAttribute : Attribute
            where TSessionSaver : BaseSessionSaver
        ;
    }
}
