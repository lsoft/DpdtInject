using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DpdtInject.Injector.Helper
{
    /*

    public static class DictionaryHelper
    {
        public static Dictionary<TKey2, TValue2> ToDictionary<TKey1, TValue1, TKey2, TValue2>(
            this IReadOnlyDictionary<TKey1, TValue1> dict,
            Func<TKey1, TKey2> keyConverter,
            Func<TValue1, TValue2> valueConverter
            )
            where TKey1 : notnull
            where TKey2 : notnull
        {
            if (dict is null)
            {
                throw new ArgumentNullException(nameof(dict));
            }

            if (keyConverter is null)
            {
                throw new ArgumentNullException(nameof(keyConverter));
            }

            if (valueConverter is null)
            {
                throw new ArgumentNullException(nameof(valueConverter));
            }

            var result = new Dictionary<TKey2, TValue2>();

            foreach(var pair in dict)
            {
                result.Add(
                    keyConverter(pair.Key),
                    valueConverter(pair.Value)
                    );
            }

            return result;
        }
    }


    //*/
}
