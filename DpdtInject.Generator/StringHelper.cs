using System;
using System.Collections.Generic;

namespace DpdtInject.Generator
{
    public static class StringHelper
    {
        public static string CheckAndReplace(
            this string source,
            string oldValue,
            string newValue
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (oldValue is null)
            {
                throw new ArgumentNullException(nameof(oldValue));
            }

            if (newValue is null)
            {
                throw new ArgumentNullException(nameof(newValue));
            }

            if(!source.Contains(oldValue))
            {
                throw new InvalidOperationException($"Source string does not contains [{oldValue}]");
            }

            return
                source.Replace(oldValue, newValue);
        }

        public static string ConvertMinusToGround(
            this string s
            )
        {
            return s.Replace('-', '_');
        }

        public static string FormatAdv(
            this string root,
            params string[] args
            )
        {
            if (string.IsNullOrEmpty(root))
            {
                throw new ArgumentException($"'{nameof(root)}' cannot be null or empty", nameof(root));
            }

            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            var result = root;

            var index = 0;
            foreach(var arg in args)
            {
                result = result.Replace($"<<{index}>>", arg);
            }

            return result;
        }
    }
}
