namespace Fanex.NCB.Core.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

public static class LinqExtension
{
    public static IEnumerable<TSource> CustomeDistinctBy<TSource, TKey>
        (this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
    {
        HashSet<TKey> seenKeys = new();
        return from TSource element in source
            where seenKeys.Add(keySelector(element))
            select element;
    }
}