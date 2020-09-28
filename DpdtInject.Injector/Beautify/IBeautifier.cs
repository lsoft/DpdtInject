using DpdtInject.Injector.Module.CustomScope;
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





        TRequestedType Get<TRequestedType>(CustomScopeObject? scope = null);


        List<TRequestedType> GetAll<TRequestedType>(CustomScopeObject? scope = null);




        object Get(Type requestedType, CustomScopeObject? scope = null);


        IEnumerable<object> GetAll(Type requestedType, CustomScopeObject? scope = null);

    }
}