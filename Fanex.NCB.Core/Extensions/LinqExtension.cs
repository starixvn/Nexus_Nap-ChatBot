namespace Fanex.NCB.Core.Extensions
{
    using System;
    using System.Collections.Generic;

    public static class LinqExtension
    {
        public static IEnumerable<TSource> CustomeDistinctBy<TSource, TKey>
    (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }
    }
}