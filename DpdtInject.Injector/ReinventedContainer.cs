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

        public ReinventedContainer(
            params Tuple<Type, Func<object>>[] pairs
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

            _length = GetPower2Length(pairs.Length);
            _mask = _length - 1;

            var preTable = new List<HashTuple>[_length];
            foreach (var pair in pairs)
            {
                var type = pair.Item1;
                var func = pair.Item2;

                var index = CalculateIndex(type);

                if(preTable[index] is null)
                {
                    preTable[index] = new List<HashTuple>();
                }

                preTable[index].Add(
                    new HashTuple(type, func)
                    );
            }

            _table = new HashTuple[_length][];
            for (var i = 0; i < preTable.Length; i++)
            {
                if (preTable[i] is null)
                {
                    _table[i] = new HashTuple[0];
                }
                else
                {
                    _table[i] = preTable[i].ToArray();
                }
            }
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
