using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DpdtInject.Injector.Src.Helper;

namespace DpdtInject.Injector.Src.Reinvented
{
    /// <summary>
    /// Special thanks to neuecc from https://github.com/neuecc for amazing idea.
    /// </summary>
    public class FlexibleSizeObjectContainer :
        IDisposable
#if !DPDT_INTERNAL_SUPPRESS_ASYNC_DISPOSABLE
        , IAsyncDisposable
#endif
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
            if (_table.Length == 0)
            {
                return;
            }

            List<Exception>? excps = null;

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
                            excps ??= new List<Exception>();
                            excps.Add(excp);
                        }
                    }
                }
            }

            if(excps != null && excps.Count > 0)
            {
                throw new AggregateException(excps);
            }

            GC.SuppressFinalize(this);
        }


#if !DPDT_INTERNAL_SUPPRESS_ASYNC_DISPOSABLE
        public async ValueTask DisposeAsync()
        {
            if (_table.Length == 0)
            {
                return;
            }

            List<Exception>? excps = null;

            for (var r = 0; r < _table.Length; r++)
            {
                var row = _table[r];
                for (var c = 0; c < row.Count; c++)
                {
                    try
                    {
                        if (row[c].Object is IAsyncDisposable ad)
                        {
                            await ad.DisposeAsync().ConfigureAwait(false);
                        }
                        //only if target is only sync disposable, do it!
                        else if (row[c].Object is IDisposable d)
                        {
                            d.Dispose();
                        }
                    }
                    catch (Exception excp)
                    {
                        excps ??= new List<Exception>();
                        excps.Add(excp);
                    }
                }
            }

            if (excps != null && excps.Count > 0)
            {
                throw new AggregateException(excps);
            }

            GC.SuppressFinalize(this);
        }
#endif


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
