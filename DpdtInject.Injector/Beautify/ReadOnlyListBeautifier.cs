using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Injector.Beautify
{
    public sealed class ReadOnlyListBeautifier : IReadOnlyListBeautifier
    {
        private readonly IBeautifier _beautifier;

        public ReadOnlyListBeautifier(
            IBeautifier beautifier
            )
        {
            if (beautifier is null)
            {
                throw new ArgumentNullException(nameof(beautifier));
            }

            _beautifier = beautifier;
        }

        public bool IsRegisteredFrom<T>()
        {
            return _beautifier.IsRegisteredFrom<T>();
        }

        public T Get<T>(
            )
        {
            return _beautifier.Get<T>();
        }

        public IReadOnlyList<T> GetAll<T>(
            )
        {
            return _beautifier.GetAll<T>();
        }

        public object Get(
            Type requestedType
            )
        {
            return _beautifier.Get(requestedType);
        }

        public IReadOnlyList<object> GetAll(
            Type requestedType
            )
        {
            return _beautifier.GetAll(requestedType).ToList();
        }
    }

}
