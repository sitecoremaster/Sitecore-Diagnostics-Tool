namespace Sitecore.DiagnosticsTool.Core.Base
{
  using System;

  /// <summary>
  ///   This class cannot be instantiated and should be always treate as null.
  /// </summary>
  public class Null
  {
    private Null()
    {
      throw new NotSupportedException();
    }
  }
}