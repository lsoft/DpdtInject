using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Beautify
{
    public interface IReadOnlyListBeautifier
    {
        bool IsRegisteredFrom<T>();


        T Get<T>(CustomScopeObject? scope = null);
        IReadOnlyList<T> GetAll<T>(CustomScopeObject? scope = null);


        object Get(Type requestedType, CustomScopeObject? scope = null);
        IReadOnlyList<object> GetAll(Type requestedType, CustomScopeObject? scope = null);
    }
}