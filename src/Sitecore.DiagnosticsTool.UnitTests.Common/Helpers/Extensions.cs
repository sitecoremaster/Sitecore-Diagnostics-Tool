namespace Sitecore.DiagnosticsTool.UnitTests.Common.Helpers
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public static class Extensions
  {
    public static void Invoke<T>([NotNull] this IEnumerable<T> enumerable)
    {
      Assert.ArgumentNotNull(enumerable, nameof(enumerable));

      // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
      enumerable.ToArray();
    }
  }
}