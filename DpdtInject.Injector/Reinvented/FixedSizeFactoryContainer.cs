using DpdtInject.Injector.Excp;
using DpdtInject.Injector.Helper;
using DpdtInject.Injector.Module.RContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DpdtInject.Injector.Reinvented
{
    /// <summary>
    /// Special thanks to neuecc from https://github.com/neuecc for amazing idea.
    /// </summary>
    public sealed class FixedSizeFactoryContainer
    {
        private readonly int _length;
        private readonly int _mask;
        private readonly HashTupleFunc[][] _table;

        public FixedSizeFactoryContainer(
            params Tuple<Type, Func<IResolutionRequest, object>>[] pairs
            )
        {
            if (pairs is null)
            {
                throw new ArgumentNullException(nameof(pairs));
            }

            #region check for duplicate (for additional safety)

            var duplicateChecker = new HashSet<Type>();
            foreach (var pair in pairs)
            {
                var type = pair.Item1;

                if (duplicateChecker.Contains(type))
                {
                    throw new DpdtException(
                        DpdtExceptionTypeEnum.InternalError,
                        $"Duplicate types incomes into the container: [{type.FullName}]"
                        );
                }
                duplicateChecker.Add(type);
            }

            #endregion

            _length = MathHelper.GetPower2Length(pairs.Length);
            _mask = _length - 1;

            var preTable = new List<HashTupleFunc>[_length];
            foreach (var pair in pairs)
            {
                var type = pair.Item1;
                var func = pair.Item2;

                var index = CalculateIndex(type);

                if (preTable[index] is null)
                {
                    preTable[index] = new List<HashTupleFunc>();
                }

                preTable[index].Add(
                    new HashTupleFunc(type, func)
                    );
            }

            _table = new HashTupleFunc[_length][];
            for (var i = 0; i < preTable.Length; i++)
            {
                if (preTable[i] is null)
                {
                    _table[i] = new HashTupleFunc[0];
                }
                else
                {
                    _table[i] = preTable[i].ToArray();
                }
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? GetGetObject(
            Type requestedType,
            IResolutionRequest resolutionRequest
            )
        {
            var index = CalculateIndex(requestedType);

            if (_table.Length > index)
            {
                var list = _table[index];

                for (var i = 0; i < list.Length; i++)
                {
                    var item = list[i];

                    if (item.Type == requestedType)
                    {
                        return item.Factory(resolutionRequest);
                    }
                }
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.NoBindingAvailable,
                $"No bindings available for {requestedType.FullName}",
                requestedType.FullName!
                );
        }

        public bool IsRegisteredFrom(Type requestedType)
        {
            var index = CalculateIndex(requestedType);

            if (_table.Length > index)
            {
                var list = _table[index];

                for (var i = 0; i < list.Length; i++)
                {
                    var item = list[i];

                    if (item.Type == requestedType)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateIndex(Type type)
        {
            var hashCode = type.GetHashCode();
            var index = hashCode & _mask;
            return index;
        }

        public struct HashTupleFunc
        {
            public readonly Type Type;
            public readonly Func<IResolutionRequest, object> Factory;

            public HashTupleFunc(Type type, Func<IResolutionRequest, object> factory)
            {
                Type = type;
                Factory = factory;
            }
        }

    }

}
