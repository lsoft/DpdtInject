using DpdtInject.Injector.Excp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DpdtInject.Injector
{
    /// <summary>
    /// Special thanks to neuecc from https://github.com/neuecc for amazing idea.
    /// </summary>
    public class ReinventedContainer
    {
        public struct HashTuple
        {
            public Type Type;
            public Func<object> Factory;

            public HashTuple(Type type, Func<object> factory)
                //: this()
            {
                Type = type;
                Factory = factory;
            }

            //public HashTuple()
            //{
            //    Type = typeof(NonUsedClass);
            //    Factory = null;
            //}
        }

        private readonly int _length;
        private readonly int _mask;
        private readonly HashTuple[][] _table;
        private readonly int[] _tableIndexes;
        private readonly HashSet<Type> _knownTypes;

        public ReinventedContainer(
            params Tuple<Type, Func<object>>[] pairs
            )
        {
            _length = GetPower2Length(pairs.Length);
            _mask = _length - 1;
            _table = new HashTuple[_length][];
            _tableIndexes = new int[_length];
            _knownTypes = new HashSet<Type>();

            for(var index = 0; index < _length; index++)
            {
                _table[index] = new HashTuple[3]; //TODO: move as constructor argument and calculate appropriate at the compilation stage
            }

            foreach (var pair in pairs)
            {
                var type = pair.Item1;
                var func = pair.Item2;

                var index = CalculateIndex(type);

                _table[index][_tableIndexes[index]] = new HashTuple(type, func);
                _knownTypes.Add(type);
                _tableIndexes[index]++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsTypeKnown(Type requestedType)
        {
            return _knownTypes.Contains(requestedType);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<HashTuple> GetGetAllDirty(Type requestedType)
        {
            if (requestedType is null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            var result = new List<Func<object>>();

            var index = CalculateIndex(requestedType);

            var list = _table[index];

            return list.ToList(); //TODO: refactor, it's too slow

            //if (list is null)
            //{
            //    return result;
            //}

            //for (var i = 0; i < list.Count; i++)
            //{
            //    var item = list[i];

            //    if (item.Type == requestedType)
            //    {
            //        result.Add(item.Factory);
            //    }
            //}

            //return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object? GetGetObject(Type requestedType)
        {
            var index = CalculateIndex(requestedType);

            var list = _table[index];

            for (var i = 0; i < list.Length; i++)
            {
                var item = list[i];

                if (item.Type == requestedType)
                {
                    return item.Factory();
                }
            }

            throw new DpdtException(
                DpdtExceptionTypeEnum.NoBindingAvailable,
                $"No bindings available for {requestedType.FullName}",
                requestedType.FullName!
                );
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateIndex(Type type)
        {
            var hashCode = type.GetHashCode();
            var index = hashCode & _mask;
            return index;
        }

        private static int GetPower2Length(int number)
        {
            var log = Math.Log(number, 2);
            var power = (int)Math.Ceiling(log);

            return (int)Math.Pow(2, power);
        }
    }

}
