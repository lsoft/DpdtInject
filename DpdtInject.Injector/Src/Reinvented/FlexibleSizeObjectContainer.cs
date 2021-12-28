﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Injector.Src.Reinvented
{
    /// <summary>
    /// Special thanks to neuecc from https://github.com/neuecc for amazing idea.
    /// </summary>
    public class FlexibleSizeObjectContainer : IDisposable
    {
        private readonly int _length;
        private readonly int _mask;
        private readonly List<HashTupleObject>[] _table;

        public FlexibleSizeObjectContainer(
            int estimatedTypeCount,
            int estimatedObjectCountPerHash = 3
            )
        {
            if(estimatedTypeCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedTypeCount));
            }
            if (estimatedObjectCountPerHash < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(estimatedObjectCountPerHash));
            }

            _length = MathHelper.GetPower2Length(estimatedTypeCount);
            _mask = _length - 1;

            _table = new List<HashTupleObject>[_length];
            for (var i = 0; i < _length; i++)
            {
                _table[i] = new List<HashTupleObject>(estimatedObjectCountPerHash);
            }
        }

        public object GetOrAdd(
            Guid uniqueId,
            Func<object> o
            )
        {
            var index = CalculateIndex(uniqueId);

            var list = _table[index];

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (item.UniqueId == uniqueId)
                {
                    return item.Object;
                }
            }

            var obj = o();

            list.Add(
                new HashTupleObject(uniqueId, obj)
                );

            return obj;
        }


        public void Dispose()
        {
            var excps = new List<Exception>();

            for(var r = 0; r < _table.Length; r++)
            {
                var row = _table[r];
                for (var c = 0; c < row.Count; c++)
                {
                    if (row[c].Object is IDisposable d)
                    {
                        try
                        {
                            d.Dispose();
                        }
                        catch(Exception excp)
                        {
                            excps.Add(excp);
                        }
                    }
                }
            }

            if(excps.Count > 0)
            {
                throw new AggregateException(excps);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int CalculateIndex(Guid uniqueId)
        {
            var hashCode = uniqueId.GetHashCode();
            var index = hashCode & _mask;
            return index;
        }


        private sealed class HashTupleObject
        {
            public readonly Guid UniqueId;
            public readonly object Object;

            public HashTupleObject(Guid uniqueId, object @object)
            {
                UniqueId = uniqueId;
                Object = @object;
            }
        }
    }
}
