using System;
using System.Collections.Generic;
using System.Linq;

namespace DpdtInject.Injector.Beautify
{
    public sealed class ListBeautifier : IListBeautifier
    {
        private readonly IBeautifier _beautifier;

        public ListBeautifier(
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

        public List<T> GetAll<T>(
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

        public List<object> GetAll(
            Type requestedType
            )
        {
            return _beautifier.GetAll(requestedType).ToList();
        }
    }

}
