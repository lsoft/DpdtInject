using System;
using System.Collections.Generic;

namespace DpdtInject.Injector.Beautify
{
    public interface IBeautifier
    {
        IListBeautifier List
        {
            get;
        }

        IReadOnlyListBeautifier ReadOnlyList
        {
            get;
        }

        bool IsRegisteredFrom<TRequestedType>();


        TRequestedType Get<TRequestedType>();
        List<TRequestedType> GetAll<TRequestedType>();


        object Get(Type requestedType);
        IEnumerable<object> GetAll(Type requestedType);

    }
}