// Copyright © 2023 Simon Condon.
// You may use this file in accordance with the terms of the MIT license.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SCSetTrie
{
    internal static class IComparerExtensions
    {
        // TODO-BREAKING: Could do something like this - but honestly don't want to have
        // to explain how not providing a comparer is okay only if your type is comparable
        // (with well-behaved comparison), *or* if you're not doing any kind of persistence.
        // Its too.. arm-wavy. Won't give people confidence. Maybe I should just only
        // allow default comparer for comaprable types (but make hash code one available)
        ////public static IComparer<T> MakeDefaultComparer<T>()
        ////{
        ////    if (typeof(IComparable<T>).IsAssignableFrom(typeof(T)) || typeof(IComparable).IsAssignableFrom(typeof(T)))
        ////    {
        ////        return Comparer<T>.Default;
        ////    }
        ////    else
        ////    {
        ////        return new CollisionResolvingHashCodeComparer<T>();
        ////    }
        ////}

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
        ////    // can make the below slightly more performant...
        ////    var lastElement = enumerable.First();
        ////    foreach (var element in enumerable.Skip(1))
        ////    {
        ////        if (comparer.Compare(lastElement, element) == 0)
        ////        {
        ////            return true;
        ////        }
        ////
        ////        lastElement = element;
        ////    }
        ////
        ////    return false;
        ////}
    }
}
