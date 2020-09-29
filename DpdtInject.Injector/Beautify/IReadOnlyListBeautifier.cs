using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Beautify
{
    public interface IReadOnlyListBeautifier
    {
        bool IsRegisteredFrom<T>();


        T Get<T>();
        IReadOnlyList<T> GetAll<T>();


        object Get(Type requestedType);
        IReadOnlyList<object> GetAll(Type requestedType);
    }
}