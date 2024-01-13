// Copyright © 2023-2024 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCSetTrie
{
    internal static class IComparerExtensions
    {
        public static T[] Sort<T>(this IComparer<T> comparer, ISet<T> set)
        {
            var elements = set.ToArray();
            Array.Sort(elements, comparer);

            ////Debug.Assert(
            ////    !HasComparisonsOfZero(comparer, set),
            ////    "Key contains at least one element pair for which the element comparer gives a comparison of zero. The element comparer is unsuitable for use by a set trie.");

            return elements;
        }

        // Probably ultimately want to allow working with IEnumerable<T>.
        // But in that case probably want to check (and not just in Debug) that there aren't duplicates.
        // Or do we always check, even with set types, to verify comparer isn't innappropriate? Have to
        // sort anyway, so not much of an extra load to check adjacent pairs as we do so?
        // Could wrap comparer in comparer that throws on zero rather than iterating afterwards?
        // Fail fast, but suspect that'd be a fair amount slower on the happy path. Could test..
        ////public static T[] SortAndValidateUnambiguousOrdering<T>(this IComparer<T> comparer, IEnumerable<T> key)
        ////{
        ////    var keyElements = key.ToArray();
        ////    Array.Sort(keyElements, comparer);
        ////
        ////    if (HasComparisonsOfZero(comparer, key))
        ////    {
        ////        throw new ArgumentException("Key contains at least one element pair for which the element comparer gives a comparison of zero. " +
        ////            "Either this pair are duplicates (meaning the passed value is not a valid set), or the element comparer is unsuitable for use by a set trie.");
        ////    }
        ////
        ////    return keyElements;
        ////}

        ////private static bool HasComparisonsOfZero<T>(this IComparer<T> comparer, IEnumerable<T> enumerable)
        ////{
        ////    using var enumerator = enumerable.GetEnumerator();

        ////    if (!enumerator.MoveNext())
        ////    {
        ////        return false;
        ////    }

        ////    var lastElement = enumerator.Current;
        ////    while (enumerator.MoveNext())
        ////    {
        ////        if (comparer.Compare(lastElement, enumerator.Current) == 0)
        ////        {
        ////            return true;
        ////        }

        ////        lastElement = enumerator.Current;
        ////    }

        ////    return false;
        ////}
    }
}
