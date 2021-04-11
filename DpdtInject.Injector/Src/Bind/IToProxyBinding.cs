using System;

namespace DpdtInject.Injector.Src.Bind
{
    public interface IToProxyBinding
    {
        IScopeBinding WithProxySettings<TAttribute, TSessionSaver>()
            where TAttribute : Attribute
            where TSessionSaver : BaseSessionSaver
        ;
    }
}
