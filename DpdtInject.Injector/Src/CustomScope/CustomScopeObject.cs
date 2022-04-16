using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using DpdtInject.Injector.Src.Reinvented;

namespace DpdtInject.Injector.Src.CustomScope
{
    public readonly struct CustomScopeObject : 
        IEquatable<CustomScopeObject>
        ,IDisposable
#if !DPDT_INTERNAL_SUPPRESS_ASYNC_DISPOSABLE
        ,IAsyncDisposable
#endif
    {
        public static readonly CustomScopeObject None = new CustomScopeObject(0);

        private readonly FlexibleSizeObjectContainer? _dependencyContainer;

        public CustomScopeObject(int estimatedTypeCount)
        {
            if (estimatedTypeCount > 0)
            {
                _dependencyContainer = new FlexibleSizeObjectContainer(
                    estimatedTypeCount
                    );
            }
            else
            {
                _dependencyContainer = null;
            }
        }

        /// <summary>
        /// Internal use only. Please DO NOT use this method.
        /// </summary>
        public object GetOrAdd(
            Guid uniqueId,
            Func<object> objectProvider
            )
        {
            if (_dependencyContainer is null)
            {
                throw new InvalidOperationException("It's an empty custom scope object!");
            }

            var result = _dependencyContainer.GetOrAdd(uniqueId, objectProvider);

            return result;
        }

        public void Dispose()
        {
            _dependencyContainer?.Dispose();
        }

#if !DPDT_INTERNAL_SUPPRESS_ASYNC_DISPOSABLE
        public ValueTask DisposeAsync()
        {
            return _dependencyContainer?.DisposeAsync() ?? ValueTask.CompletedTask;
        }
#endif

        public bool Equals(CustomScopeObject other) => ReferenceEquals(_dependencyContainer, other._dependencyContainer);

        public override bool Equals([NotNullWhen(true)] object? other) => other is CustomScopeObject && Equals((CustomScopeObject)other);
        
        public override int GetHashCode() => _dependencyContainer?.GetHashCode() ?? 0;

        public static bool operator ==(CustomScopeObject left, CustomScopeObject right) => left.Equals(right);

        public static bool operator !=(CustomScopeObject left, CustomScopeObject right) => !left.Equals(right);
    }
}
