namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Logging
{
  using System;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.LogAnalyzer.Models;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;

  public class LogEntryEx : ILogEntry
  {
    [PublicAPI]
    internal LogEntryEx()
    {
      Thread = string.Empty;
      Date = DateTime.MinValue;
      Level = Core.Resources.Logging.LogLevel.All;
      Message = string.Empty;
      Exception = null;
      RawText = string.Empty;
    }

    [PublicAPI]
    public LogEntryEx(DateTime moment, Core.Resources.Logging.LogLevel level, [NotNull] string message, ExceptionInfo exception)
    {
      Assert.ArgumentNotNull(message, nameof(message));

      Thread = string.Empty;
      Date = moment;
      Level = level;
      Message = message;
      Exception = exception;
      RawText = moment + " " + level + " " + message + ("\r\n" + exception).TrimEnd("\r\n".ToCharArray());
    }

    public LogEntryEx([NotNull] LogEntry logEntry)
    {
      Assert.ArgumentNotNull(logEntry, nameof(logEntry));

      var level = logEntry.Level;
      Assert.IsNotNull(level.ToString(), "level");

      Thread = logEntry.EventSource;
      Date = logEntry.LogDateTime;
      Level = ParseLevel(level);
      Message = logEntry.Error.With(x => x.Message) ?? logEntry.Caption ?? string.Empty;
      Exception = ParseException(logEntry.Error);
      RawText = logEntry.ToString();
    }

    public string Thread { get; internal set; }

    public DateTime Date { get; internal set; }

    public Core.Resources.Logging.LogLevel Level { get; internal set; }

    public string Message { get; internal set; }

    public ExceptionInfo Exception { get; internal set; }

    public string RawText { get; internal set; }

    private static ExceptionInfo ParseException(SmartErrorDetails error)
    {
      if (error == null || !error.HasException)
      {
        return null;
      }

      return new ExceptionInfo(TypeRef.Parse(error.Exception), error.Message, error.StackTrace, ParseException(error.NestedException));
    }

    private static Core.Resources.Logging.LogLevel ParseLevel(Diagnostics.LogAnalyzer.Models.LogLevel level)
    {
      Assert.ArgumentNotNullOrEmpty(level.ToString(), nameof(level));

      switch (level.ToString())
      {
        case "FATAL":
          return Core.Resources.Logging.LogLevel.Fatal;

        case "CRITICAL":
          return Core.Resources.Logging.LogLevel.Critical;

        case "ERROR":
          return Core.Resources.Logging.LogLevel.Error;

        case "WARN":
          return Core.Resources.Logging.LogLevel.Warn;

        case "INFO":
          return Core.Resources.Logging.LogLevel.Info;

        case "DEBUG":
          return Core.Resources.Logging.LogLevel.Debug;

        default:
          throw new NotImplementedException($"The {level} type is not implemented to be recognized");
      }
    }
  }
}