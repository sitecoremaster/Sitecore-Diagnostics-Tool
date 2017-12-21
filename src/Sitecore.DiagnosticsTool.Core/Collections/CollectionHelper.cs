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

    public static bool AreIdentical<TKey, TValue>([NotNull] IReadOnlyDictionary<TKey, TValue> first, [NotNull] IReadOnlyDictionary<TKey, TValue> second, [NotNull] Func<TKey, TKey, bool> keyComparer, [NotNull] Func<TValue, TValue, bool> valueComparer) where TValue : class
    {
      var keys1 = first.Keys.OrderBy(x => x.GetHashCode()).ToList();
      var keys2 = second.Keys.OrderBy(x => x.GetHashCode()).ToList();

      var result = true;
      var arr1 = keys1.ToArray();
      var arr2 = keys2.ToArray();
      foreach (var key1 in arr1)
      {
        foreach (var key2 in arr2)
        {
          if (keyComparer(key1, key2))  
          {
            keys1.Remove(key1);
            keys2.Remove(key2);

            if (!valueComparer(first.TryGetValue(key1), second.TryGetValue(key1)))
            {
              result = false;
            }
          }
        }
      }

      foreach (var key in keys1)
      {
        valueComparer(first.TryGetValue(key), null);
      }

      foreach (var key in keys2)
      {
        valueComparer(null, second.TryGetValue(key));
      }

      return result;
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