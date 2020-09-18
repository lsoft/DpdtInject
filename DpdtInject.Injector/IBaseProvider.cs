using System;
using System.Collections.Generic;

namespace DpdtInject.Injector
{
    public interface IBaseProvider<T>
    {
        Func<T> GetFunc();

        T Get();

        List<T> GetAll();
    }
}
