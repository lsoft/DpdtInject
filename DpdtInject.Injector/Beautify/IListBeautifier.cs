using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Beautify
{
    public interface IListBeautifier
    {
        bool IsRegisteredFrom<T>();

        T Get<T>();
        List<T> GetAll<T>();


        object Get(Type requestedType);
        
        List<object> GetAll(Type requestedType);
    }
}