using System;
using DpdtInject.Injector.Src.Reinvented;

namespace DpdtInject.Injector.Src.CustomScope
{
    public sealed class CustomScopeObject : IDisposable
    {
        private readonly FlexibleSizeObjectContainer _dependencyContainer;

        public CustomScopeObject(int estimatedTypeCount)
        {
            _dependencyContainer = new FlexibleSizeObjectContainer(
                estimatedTypeCount
                );
        }

        /// <summary>
        /// Internal use only. Please DO NOT use this method.
        /// </summary>
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
