using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Module.CustomScope;
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

        FixedSizeFactoryContainerCustomScope TypeContainerGetCustomScope
        {
            get;
        }

        FixedSizeFactoryContainer TypeContainerGetAll
        {
            get;
        }

        FixedSizeFactoryContainerCustomScope TypeContainerGetAllCustomScope
        {
            get;
        }
    }


    public interface IBindingProvider<T>
    {
        T Get();

        T Get(CustomScopeObject scope);

        List<T> GetAll();

        List<T> GetAll(CustomScopeObject scope);
    }
}
