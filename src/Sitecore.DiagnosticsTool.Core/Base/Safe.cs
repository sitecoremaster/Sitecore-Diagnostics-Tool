namespace Sitecore.DiagnosticsTool.Core.Base
{
  using System;

  public static class Safe
  {
    public static T Run<T>(Func<Null, T> value) where T : class
    {
      try
      {
        return value(null);
      }
      catch
      {
        return null;
      }
    }
  }
}
