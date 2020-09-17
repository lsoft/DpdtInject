﻿using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Beautify
{
    public class Beautifier
    {
        public FakeModule Module
        {
            get;
        }

        public Beautifier.ListBeautifier List
        {
            get;
        }
        public Beautifier.ReadOnlyListBeautifier ReadOnlyList
        {
            get;
        }

        public Beautifier(
            FakeModule module
            )
        {
            if (module is null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            Module = module;
            List = new Beautifier.ListBeautifier(this);
            ReadOnlyList = new Beautifier.ReadOnlyListBeautifier(this);
        }

        public T Get<T>()
        {
            try
            {
                return Module.Get<T>();
            }
            catch (InvalidCastException)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for [{typeof(T).FullName}]",
                    typeof(T).FullName!
                    );
            }
        }

        public List<T> GetAll<T>()
        {
            try
            {
                return Module.GetAll<T>();
            }
            catch (InvalidCastException)
            {
                throw new DpdtException(
                    DpdtExceptionTypeEnum.NoBindingAvailable,
                    $"No bindings available for [{typeof(T).FullName}]",
                    typeof(T).FullName!
                    );
            }
        }

        public object Get(Type requestedType)
        {
            return Module.Get(requestedType);
        }

        public IEnumerable<object> GetAll(Type requestedType)
        {
            return Module.GetAll(requestedType);
        }

        public class ReadOnlyListBeautifier
        {
            private readonly Beautifier _beautifier;

            public ReadOnlyListBeautifier(
                Beautifier beautifier
                )
            {
                if (beautifier is null)
                {
                    throw new ArgumentNullException(nameof(beautifier));
                }

                _beautifier = beautifier;
            }

            public T Get<T>()
            {
                return _beautifier.Get<T>();
            }

            public IReadOnlyList<T> GetAll<T>()
            {
                return _beautifier.GetAll<T>();
            }

            public object Get(Type requestedType)
            {
                return _beautifier.Get(requestedType);
            }

            public IReadOnlyList<object> GetAll(Type requestedType)
            {
                return _beautifier.GetAll(requestedType).ToList();
            }
        }

        public class ListBeautifier
        {
            private readonly Beautifier _beautifier;

            public ListBeautifier(
                Beautifier beautifier
                )
            {
                if (beautifier is null)
                {
                    throw new ArgumentNullException(nameof(beautifier));
                }

                _beautifier = beautifier;
            }

            public T Get<T>()
            {
                return _beautifier.Get<T>();
            }

            public List<T> GetAll<T>()
            {
                return _beautifier.GetAll<T>();
            }

            public object Get(Type requestedType)
            {
                return _beautifier.Get(requestedType);
            }

            public List<object> GetAll(Type requestedType)
            {
                return _beautifier.GetAll(requestedType).ToList();
            }
        }
    }
}
