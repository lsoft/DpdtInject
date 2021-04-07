using System;
using System.Collections.Generic;

namespace DpdtInject.Generator.Core.Helpers
{
    public static class ListHelper
    {
        public static IEnumerable<(T, bool)> IterateWithLastSignal<T>(
            this IReadOnlyList<T> t
            )
        {
            if (t.Count == 0)
            {
                yield break;
            }

            for (var i = 0; i < t.Count - 1; i++)
            {
                yield return (t[i], false);
            }

            yield return (t[t.Count - 1], true);
        }

        public static void IterateWithLastSignal<T>(
            this IReadOnlyList<T> t,
            Action<T> nonLastAction,
            Action<T> lastAction
            )
        {
            if (t.Count == 0)
            {
                return;
            }

            for (var i = 0; i < t.Count - 1; i++)
            {
                nonLastAction(t[i]);
            }

            lastAction(t[t.Count - 1]);
        }

        public static void IterateWithLastSignal<T>(
            this IReadOnlyList<T> t,
            Action<T> mainAction,
            Action betweenAction
            )
        {
            if (t.Count == 0)
            {
                return;
            }

            for (var i = 0; i < t.Count - 1; i++)
            {
                mainAction(t[i]);
                betweenAction();
            }

            mainAction(t[t.Count - 1]);
        }

    }

}
