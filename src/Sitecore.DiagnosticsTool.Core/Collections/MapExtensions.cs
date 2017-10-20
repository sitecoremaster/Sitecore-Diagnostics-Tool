namespace Sitecore.DiagnosticsTool.Core.Collections
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;

  public static class MapExtensions
  {
    public static Map ToMap<TSource>([NotNull] this IEnumerable<TSource> arr, [NotNull] Func<TSource, string> keySelector, [NotNull] Func<TSource, string> elementSelector)
    {
      return new Map(arr.ToDictionary(keySelector, elementSelector));
    }

    public static Map<TValue> ToMap<TSource, TValue>([NotNull] this IEnumerable<TSource> arr, [NotNull] Func<TSource, string> keySelector, [NotNull] Func<TSource, TValue> elementSelector)
    {
      return new Map<TValue>(arr.ToDictionary(keySelector, elementSelector));
    }
  }
}