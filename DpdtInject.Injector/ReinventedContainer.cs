using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DpdtInject.Injector
{
    /// <summary>
    /// Special thanks to neuecc from https://github.com/neuecc for amazing idea.
    /// </summary>
    public class ReinventedContainer
    {
        public class HashTuple
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
        private readonly List<HashTuple>[] _table;

        public ReinventedContainer(
            params (Type, Func<object>)[] pairs
            )
        {
            _length = GetPower2Length(pairs.Length);
            _table = new List<HashTuple>[_length];

            foreach (var pair in pairs)
            {
                var type = pair.Item1;
                var func = pair.Item2;

                var index = CalculateIndex(type);

                if (_table[index] is null)
                {
                    _table[index] = new List<HashTuple>();
                }

                _table[index].Add(new HashTuple(type, func));
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public List<HashTuple> GetGetAllDirty(Type requestedType)
        {
            if (requestedType is null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            //var result = new List<Func<object>>();

            var index = CalculateIndex(requestedType);

            var list = _table[index];

            return list;

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
        public Func<object>? GetGet(Type requestedType)
        {
            if (requestedType is null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            var index = CalculateIndex(requestedType);

            var list = _table[index];

            if (list is null)
            {
                return null;
            }

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (item.Type == requestedType)
                {
                    return item.Factory;
                }
            }

            return null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateIndex(Type type)
        {
            var hashCode = type.GetHashCode();
            var index = Math.Abs(hashCode) % _length;
            return index;
        }

        private static int GetPower2Length(int number)
        {
            var log = Math.Log(number, 2);
            var power = (int)Math.Ceiling(log);

            return (int)Math.Pow(2, power);
        }

        //private class NonUsedClass { }
    }

}
