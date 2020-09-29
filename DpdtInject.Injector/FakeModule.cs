using DpdtInject.Injector.Beautify;
using DpdtInject.Injector.Module;
using DpdtInject.Injector.Module.CustomScope;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector
{
    public interface ICluster
    {
        public Type DeclaredClusterType
        {
            get;
        }

        public bool IsRootCluster
        {
            get;
        }


        bool IsRegisteredFrom<TRequestedType>();


        TRequestedType Get<TRequestedType>();
        public TRequestedType Get<TRequestedType>(CustomScopeObject scope);


        List<TRequestedType> GetAll<TRequestedType>();
        List<TRequestedType> GetAll<TRequestedType>(CustomScopeObject scope);


        object Get(Type requestedType);
        object Get(Type requestedType, CustomScopeObject scope);

        IEnumerable<object> GetAll(Type requestedType);
        IEnumerable<object> GetAll(Type requestedType, CustomScopeObject scope);
    }

    public class FakeModule  : IDisposable
    {
        public CustomScopeObject CreateCustomScope()
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public TCluster GetCluster<TCluster>()
            where TCluster : ICluster
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public bool IsRegisteredFrom<TCluster, TRequestedType>()
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public bool IsRegisteredFrom<TRequestedType>()
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public TRequestedType Get<TRequestedType>()
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public TRequestedType Get<TRequestedType>(CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public TRequestedType Get<TCluster, TRequestedType>()
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public TRequestedType Get<TCluster, TRequestedType>(CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public List<TRequestedType> GetAll<TRequestedType>()
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public List<TRequestedType> GetAll<TRequestedType>(CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public List<TRequestedType> GetAll<TCluster, TRequestedType>()
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public List<TRequestedType> GetAll<TCluster, TRequestedType>(CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }



        public object Get(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public object Get(Type requestedType, CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public object Get<TCluster>(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public object Get<TCluster>(Type requestedType, CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public IEnumerable<object> GetAll(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public IEnumerable<object> GetAll(Type requestedType, CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public IEnumerable<object> GetAll<TCluster>(Type requestedType)
        {
            throw new NotImplementedException("This method should not be executed");
        }
        public IEnumerable<object> GetAll<TCluster>(Type requestedType, CustomScopeObject scope)
        {
            throw new NotImplementedException("This method should not be executed");
        }


        public void Dispose()
        {
            throw new NotImplementedException("This method should not be executed");
        }
    }

    public class FakeModule<T>  : FakeModule, IDisposable
        where T : DpdtModule
    {
    }
}
