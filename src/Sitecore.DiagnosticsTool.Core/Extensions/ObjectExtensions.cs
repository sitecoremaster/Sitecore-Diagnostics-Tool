namespace Sitecore.DiagnosticsTool.Core.Extensions
{
  using System;

  public static class ObjectExtensions
  {
    public static TOutput PassTo<TSource, TOutput>(this TSource source, Func<TSource, TOutput> func)
    {
      return func(source);
    }
  }
}