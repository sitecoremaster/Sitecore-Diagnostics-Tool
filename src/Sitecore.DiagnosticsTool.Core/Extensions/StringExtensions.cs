namespace Sitecore.DiagnosticsTool.Core.Extensions
{
  using System;

  using JetBrains.Annotations;

  public static class StringExtensions
  {
    public static string TrimEnd([NotNull] this string source, [NotNull] string pattern, StringComparison comparison)
    {
      var count = 0;
      while (source.EndsWith(pattern, comparison))
      {
        count += 1;
      }

      var offset = pattern.Length * count;
      var result = source.Substring(0, source.Length - offset);

      return result;
    }

    public static string TrimStart([NotNull] this string source, [NotNull] string pattern, StringComparison comparison)
    {
      var count = 0;
      while (source.StartsWith(pattern, comparison))
      {
        count += 1;
      }

      var offset = pattern.Length * count;
      var result = offset >= source.Length ? string.Empty : source.Substring(offset);

      return result;
    }
  }
}
