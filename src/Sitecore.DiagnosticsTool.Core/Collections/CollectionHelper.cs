namespace Sitecore.DiagnosticsTool.Core.Collections
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;

  public class CollectionHelper
  {
    public static bool AreIdentical<T>([NotNull] IReadOnlyList<T> first, [NotNull] IReadOnlyList<T> second, [NotNull] Func<T, T, bool> comparer)
    {
      if (first.Count != second.Count)
      {
        return false;
      }

      for (var i = 0; i < first.Count; i++)
      {
        if (!comparer(first[i], second[i]))
        {
          return false;
        }
      }

      return true;
    }

    public static bool AreIdentical<TKey, TValue>([NotNull] IReadOnlyDictionary<TKey, TValue> first, [NotNull] IReadOnlyDictionary<TKey, TValue> second, [NotNull] Func<TKey, TKey, bool> keyComparer, [NotNull] Func<TValue, TValue, bool> valueComparer)
    {
      var keys = first.Keys.OrderBy(x => x.GetHashCode()).ToArray();

      if (!AreIdentical(keys, second.Keys.OrderBy(x => x.GetHashCode()).ToArray(), keyComparer))
      {
        return false;
      }

      foreach (var key in keys)
      {
        if (!valueComparer(first.TryGetValue(key), second.TryGetValue(key)))
        {
          return false;
        }
      }

      return true;
    }

    /// <summary>
    ///   Compares all pairs of collection (like bubble sort) using provided comparer.
    ///   Returns false immediately when comparer function returns false, otherwise returns true.
    /// </summary>
    public static bool AreIdenticalByPairs<T>([NotNull] IReadOnlyList<T> collection, [NotNull] Func<T, T, bool> comparer)
    {
      for (var i = 0; i < collection.Count; ++i)
      {
        for (var j = i + 1; j < collection.Count; ++j)
        {
          if (!comparer(collection[i], collection[j]))
          {
            return false;
          }
        }
      }

      return true;
    }
  }
}