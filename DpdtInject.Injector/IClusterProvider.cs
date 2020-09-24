using DpdtInject.Injector.Beautify;
using System;
using System.Collections.Generic;

namespace DpdtInject.Injector
{
    public interface IClusterProvider<T>
    {
        T GetCluster();
    }

    public interface IBindingProvider
    {
        IBeautifier Beautifier
        {
            get;
        }

        ReinventedContainer TypeContainerGet
        {
            get;
        }

        ReinventedContainer TypeContainerGetAll
        {
            get;
        }
    }


    public interface IBindingProvider<T>
    {
        T Get();

        List<T> GetAll();
    }
}
