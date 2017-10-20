// <summary>
//   The read-only interface for accessing Logging information
// </summary>

namespace Sitecore.DiagnosticsTool.Core.Resources.Logging
{
  using System;
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   The read-only interface for accessing Logging information
  /// </summary>
  public interface ILoggingContext : IResource
  {
    /// <summary>
    ///   Get all Sitecore log entries.
    /// </summary>
    /// <returns>The ordered collection of ILogEntry</returns>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IEnumerable<ILogEntry> GetSitecoreLogEntries();

    /// <summary>
    ///   Get all Sitecore log entries.
    /// </summary>
    /// <param name="logLevel">The log4net log level to be included, pipe-separated list is accepted</param>
    /// <returns>The ordered collection of ILogEntry</returns>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IEnumerable<ILogEntry> GetSitecoreLogEntries(LogLevel logLevel);

    /// <summary>
    ///   Get Sitecore log entries within the specific timeframe.
    /// </summary>
    /// <param name="logLevel">The log4net log level to be included, pipe-separated list is accepted</param>
    /// <param name="start">The start moment of timeframe</param>
    /// <param name="end">The end moment of timeframe</param>
    /// <returns>The ordered collection of ILogEntry</returns>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IEnumerable<ILogEntry> GetSitecoreLogEntries(LogLevel logLevel, DateTime start, DateTime end);
  }
}