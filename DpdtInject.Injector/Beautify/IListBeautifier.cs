using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Beautify
{
    public interface IListBeautifier
    {
        bool IsRegisteredFrom<T>();

        T Get<T>(CustomScopeObject? scope = null);
        List<T> GetAll<T>(CustomScopeObject? scope = null);


        object Get(Type requestedType, CustomScopeObject? scope = null);
        
        List<object> GetAll(Type requestedType, CustomScopeObject? scope = null);
    }
}