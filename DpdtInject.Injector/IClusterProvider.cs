using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Reinvented;
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

        FixedSizeFactoryContainer TypeContainerGet
        {
            get;
        }

        FixedSizeFactoryContainer TypeContainerGetAll
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
