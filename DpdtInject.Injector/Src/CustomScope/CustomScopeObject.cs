using DpdtInject.Injector.Reinvented;
using System;

namespace DpdtInject.Injector.CustomScope
{
    public sealed class CustomScopeObject : ICustomScopeObject, IDisposable
    {
        private readonly FlexibleSizeObjectContainer _dependencyContainer;

        public CustomScopeObject(int estimatedTypeCount)
        {
            _dependencyContainer = new FlexibleSizeObjectContainer(
                estimatedTypeCount
                );
        }

        //public bool TryGetObject(
        //    Type requestedType,
        //    [NotNullWhen(true)] out object? result
        //    )
        //{
        //    return _dependencyContainer.TryGetObject(requestedType, out result);
        //}

        public object GetOrAdd(
            Guid uniqueId,
            Func<object> objectProvider
            )
        {
            var result = _dependencyContainer.GetOrAdd(uniqueId, objectProvider);

            return result;
        }

        public void Dispose()
        {
            _dependencyContainer.Dispose();
        }
    }
}
