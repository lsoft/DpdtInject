using System;
using System.Collections.Generic;
using DpdtInject.Injector.Src.CustomScope;
using DpdtInject.Injector.Src.RContext;
using DpdtInject.Injector.Src;
using System.Threading.Tasks;

namespace DpdtInject.Tests
{
    public interface IFakeFactory<T>
    {
    }

    public interface IFakeProxy<T>
    {
    }

    public class FakeCluster<T> : ICluster
        where T : DefaultCluster
    {
        public FakeCluster(
            )
        {
        }

        public FakeCluster(
            IResolution parentResolution
            )
        {
        }

        /// <inheritdoc />
        public bool IsRegisteredFrom<T1>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsRegisteredFrom(
            System.Type requestedType
            )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public bool IsRegisteredFromRecursive<T1>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public T1 Get<T1>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public T1 Get<T1>(
            CustomScopeObject customScope
            )
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public List<T1> GetAll<T1>()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public List<T1> GetAll<T1>(
            CustomScopeObject customScope
            )
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public object Get(
            Type requestedType
            )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public object Get(
            Type requestedType,
            CustomScopeObject customScope
            )
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public IEnumerable<object> GetAll(
            Type requestedType
            )
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public IEnumerable<object> GetAll(
            Type requestedType,
            CustomScopeObject scope
            )
        {
            throw new NotImplementedException();
        }


        /// <inheritdoc />
        public T1 GetFast<T1>(
            T1 unused
            )
        {
            throw new NotImplementedException();
        }


        public T1 GetToChild<T1>(
            IResolutionRequest resolutionRequest
            )
        {
            throw new NotImplementedException();
        }

        public List<Exception> TrySafelyDispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public CustomScopeObject CreateCustomScope()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
