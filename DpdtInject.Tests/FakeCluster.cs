using DpdtInject.Injector;
using DpdtInject.Injector.CustomScope;
using System;
using System.Collections.Generic;
using DpdtInject.Injector.RContext;

namespace DpdtInject.Tests
{
    public interface IFakeFactory<T>
    {
    }


    public class FakeCluster<T> : ICluster
        where T : DefaultCluster
    {
        public FakeCluster(
            IResolution parentResolution
            )
        {

        }

        public bool IsRegisteredFrom<T1>()
        {
            throw new NotImplementedException();
        }
        public bool IsRegisteredFrom(System.Type requestedType)
        {
            throw new NotImplementedException();
        }

        public T1 Get<T1>()
        {
            throw new NotImplementedException();
        }

        public T1 Get<T1>(CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }


        public List<T1> GetAll<T1>()
        {
            throw new NotImplementedException();
        }

        public List<T1> GetAll<T1>(CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }



        public object Get(Type requestedType)
        {
            throw new NotImplementedException();
        }

        public object Get(Type requestedType, CustomScopeObject customScope)
        {
            throw new NotImplementedException();
        }



        public IEnumerable<object> GetAll(Type requestedType)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<object> GetAll(Type requestedType, CustomScopeObject scope)
        {
            throw new NotImplementedException();
        }


        public T1 GetFast<T1>(T1 unused)
        {
            throw new NotImplementedException();
        }




        public T1 GetToChild<T1>(IResolutionRequest resolutionRequest)
        {
            throw new NotImplementedException();
        }

        public List<Exception> TrySafelyDispose()
        {
            throw new NotImplementedException();
        }

        public CustomScopeObject CreateCustomScope()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
