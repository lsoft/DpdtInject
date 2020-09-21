using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Helpers
{
    public static class StringHelper
    {
        public static string CheckAndReplaceIfTrue(
            this string source,
            Func<bool> predicate,
            string oldValue,
            string newValue
            )
        {
            if(!predicate())
            {
                return source;
            }

            return CheckAndReplace(source, oldValue, newValue);
        }

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

            if (!source.Contains(oldValue))
            {
                throw new InvalidOperationException($"Source string does not contains [{oldValue}]");
            }

            return
                source.Replace(oldValue, newValue);
        }

        public static string ConvertDotLessGreatherToGround(
            this string s
            )
        {
            return s.Replace('.', '_').Replace('<', '_').Replace('>', '_');
        }

        public static string RemoveMinuses(
            this Guid g
            )
        {
            return g.ToString().Replace("-", "");
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
            foreach (var arg in args)
            {
                result = result.Replace($"<<{index}>>", arg);
            }

            return result;
        }
    }
}
