using System;
using System.Collections.Generic;

namespace DpdtInject.Injector
{
    public interface IClusterProvider<T>
    {
        T Get();

        List<T> GetAll();
    }
}
