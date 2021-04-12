using System;
using System.Text;

namespace DpdtInject.Generator.Core.Helpers
{
    public static class StringHelper
    {
        public static StringBuilder AsStringBuilder(this string s)
        {
            return new StringBuilder(s);
        }

        public static string SafeSubstring(this string s, int start, int length)
        {
            if (s is null)
            {
                throw new ArgumentNullException(nameof(s));
            }

            if (s.Length < start + length)
            {
                return s.Substring(start);
            }

            return s.Substring(start, length);
        }

        public static string ReplaceLineContains(
            this string source,
            string substring,
            string newValue
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (substring is null)
            {
                throw new ArgumentNullException(nameof(substring));
            }

            if (newValue is null)
            {
                throw new ArgumentNullException(nameof(newValue));
            }

            var lines = source.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (var l = 0; l < lines.Length; l++)
            {
                if (lines[l].Contains(substring))
                {
                    lines[l] = newValue;
                }
            }

            return
                string.Join(Environment.NewLine, lines);
        }

        public static string ReplaceLineStartsWith(
            this string source,
            string startsWith,
            string newValue
            )
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (startsWith is null)
            {
                throw new ArgumentNullException(nameof(startsWith));
            }

            if (newValue is null)
            {
                throw new ArgumentNullException(nameof(newValue));
            }

            var lines = source.Split(new[] { Environment.NewLine }, StringSplitOptions.None);

            for (var l = 0; l < lines.Length; l++)
            {
                if (lines[l].StartsWith(startsWith))
                {
                    lines[l] = newValue;
                }
            }

            return
                string.Join(Environment.NewLine, lines);
        }

        public static string CheckAndReplaceIfTrue(
            this string source,
            Func<bool> predicate,
            string oldValue,
            string newValue
            )
        {
            if (!predicate())
            {
                return source;
            }

            return source.CheckAndReplace(oldValue, newValue);
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

        public static string EscapeSpecialTypeSymbols(
            this string s
            )
        {
            return s
                .Replace(' ', '_')
                .Replace('.', '_')
                .Replace('<', '_')
                .Replace('>', '_')
                .Replace(',', '_')
                ;
        }

        //public static string RemoveMinuses(
        //    this Guid g
        //    )
        //{
        //    return g.ToString().Replace("-", "");
        //}

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
