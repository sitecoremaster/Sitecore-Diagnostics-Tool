namespace Sitecore.DiagnosticsTool.Core.Extensions
{
  using System.Collections.Generic;
  using JetBrains.Annotations;

  public static class EnumerableExtensions
  {
    public static string JoinToString([NotNull] this IEnumerable<object> arr, string separator)
    {
      return string.Join(separator ?? "", arr);
    }
  }
}