namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Logging;

  public static class LogHelperEx
  {
    [NotNull]
    public static ILogEntry Parse(DateTime date, LogLevel level, [NotNull] string message)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      return new LogEntryEx(date, level, message, null);
    }

    [NotNull]
    public static ILogEntry Parse(LogLevel level, [NotNull] string message)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      return new LogEntryEx(DateTime.MinValue, level, message, null);
    }
  }
}