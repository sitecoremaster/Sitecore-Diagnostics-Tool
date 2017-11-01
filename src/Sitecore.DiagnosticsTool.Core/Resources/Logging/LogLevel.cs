namespace Sitecore.DiagnosticsTool.Core.Resources.Logging
{
  using System;

  [Flags]
  public enum LogLevel
  {
    Debug = 1,

    Info = 2,

    Warn = 4,

    Error = 8,

    Fatal = 16,

    Critical = 32,

    All = Debug | Info | Warn | Error | Fatal | Critical
  }
}