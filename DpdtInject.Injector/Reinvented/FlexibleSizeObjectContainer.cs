using DpdtInject.Injector.Helper;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DpdtInject.Injector.Reinvented
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


        //public bool TryGetObject(
        //    Type requestedType,
        //    [NotNullWhen(true)] out object? result
        //    )
        //{
        //    var index = CalculateIndex(requestedType);
        //    var list = _table[index];

        //    for (var i = 0; i < list.Count; i++)
        //    {
        //        var item = list[i];

        //        if (item.Type == requestedType)
        //        {
        //            result = item.Object;
        //            return true;
        //        }
        //    }

        //    result = null;
        //    return false;
        //}

        //public object? GetGetObject(Type requestedType)
        //{
        //    var index = CalculateIndex(requestedType);
        //    var list = _table[index];

        //    for (var i = 0; i < list.Count; i++)
        //    {
        //        var item = list[i];

        //        if (item.Type == requestedType)
        //        {
        //            return item.Object;
        //        }
        //    }

        //    throw new DpdtException(
        //        DpdtExceptionTypeEnum.NoBindingAvailable,
        //        $"No bindings available for {requestedType.FullName}",
        //        requestedType.FullName!
        //        );
        //}

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
