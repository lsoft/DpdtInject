using System;
using System.Collections.Generic;

namespace DpdtInject.Injector
{
    public interface IBaseProvider<T>
    {
        T Get();

        List<T> GetAll();
    }
}
