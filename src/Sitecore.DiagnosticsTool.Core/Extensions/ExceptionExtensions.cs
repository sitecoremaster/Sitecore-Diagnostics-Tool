namespace Sitecore.DiagnosticsTool.Core.Extensions
{
  using System;

  public static class ExceptionExtensions
  {
    public static string PrintException(this Exception ex, bool inner = false)
    {
      return ex == null ? "" : (inner ? "\r\nInner Exception:\r\n\r\n" : "") + $"Type: {ex.GetType().FullName}\r\nMessage: {ex.Message}\r\nStackTrace: \r\n{ex.StackTrace}{PrintException(ex.InnerException, true)}";
    }
  }
}