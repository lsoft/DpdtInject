using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Helper
{
    public static class ListHelper
    {
        public static List<T> Shuffle<T>(
            this IEnumerable<T> list
            )
        {
            var rnd = new Random(
                BitConverter.ToInt32(Guid.NewGuid().ToByteArray(), 0)
                );

            var result = new List<T>(list);

            for (var i = 0; i < result.Count - 1; i++)
            {
                if (rnd.Next() >= 0.5f)
                {
                    var newIndex = rnd.Next(result.Count);

                    var tmp = result[i];
                    result[i] = result[newIndex];
                    result[newIndex] = tmp;
                }
            }

            return result;
        }

        public static List<T2> ConvertAll<T1, T2>(
            this IReadOnlyList<T1> list,
            Func<T1, T2> converter
            )
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list));
            }

            if (converter is null)
            {
                throw new ArgumentNullException(nameof(converter));
            }

            var result = new List<T2>(list.Count);

            for(var a = 0; a < list.Count; a++)
            {
                result.Add(converter(list[a]));
            }

            return result;
        }


        public static string Join<T>(
            this IEnumerable<T> list,
            Func<T, string> converter,
            string? separator = null
            )
        {
            if(separator is null)
            {
                separator = Environment.NewLine;
            }

            return string.Join(separator, list.Select(a => converter(a)));
        }

        public static bool NotIn<T>(
            this T v,
            IEnumerable<T> array
            )
        {
            return
                !array.Contains(v);
        }

        public static bool In<T>(
            this T v,
            IEnumerable<T> array
            )
        {
            return
                array.Contains(v);
        }

        public static bool NotIn<T>(
            this T v,
            params T[] array
            )
        {
            return
                !array.Contains(v);
        }

        public static bool In<T>(
            this T v,
            params T[] array
            )
        {
            return
                array.Contains(v);
        }

        public static bool NotIn<T>(
            this T v,
            IEqualityComparer<T> comparer,
            params T[] array
            )
        {
            return
                !array.Contains(v, comparer);
        }

        public static bool In<T>(
            this T v,
            IEqualityComparer<T> comparer,
            params T[] array
            )
        {
            return
                array.Contains(v, comparer);
        }

        public static IEnumerable<List<T>> Split<T>(
            this IEnumerator<T> list,
            int splitCount
            )
        {
            if (splitCount <= 0)
            {
                throw new ArgumentException("splitCount <= 0");
            }

            var nextList = new List<T>();

            while (list.MoveNext())
            {
                var item = list.Current;

                nextList.Add(item);

                if (nextList.Count == splitCount)
                {
                    yield return nextList;

                    nextList = new List<T>();
                }
            }

            //if (list.Count % splitCount != 0)
            if (nextList.Count > 0)
            {
                yield return nextList;
            }
        }

        public static IEnumerable<List<T>> Split<T>(
            this IEnumerable<T> list,
            int splitCount
            )
        {
            if (splitCount <= 0)
            {
                throw new ArgumentException("splitCount <= 0");
            }

            var nextList = new List<T>();

            foreach (var item in list)
            {
                nextList.Add(item);

                if (nextList.Count == splitCount)
                {
                    yield return nextList;

                    nextList = new List<T>();
                }
            }

            //if (list.Count % splitCount != 0)
            if (nextList.Count > 0)
            {
                yield return nextList;
            }
        }

    }

}
