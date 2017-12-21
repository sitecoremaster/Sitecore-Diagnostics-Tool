namespace Sitecore.DiagnosticsTool.Core.Resources.Logging
{
  using System;

  using JetBrains.Annotations;

  public interface ILogEntry
  {
    /// <summary>
    ///   The source thread of the log entry. Example:
    ///   <code>ManagedPoolThread #1</code>
    /// </summary>
    [NotNull]
    string Thread { get; }

    /// <summary>
    ///   The date and time of log entry. Example:
    ///   <code>23/02/2015 08:59:43</code>
    /// </summary>
    DateTime Date { get; }

    /// <summary>
    ///   The log level. Example:
    ///   <code>ERROR</code>
    /// </summary>
    LogLevel Level { get; }

    /// <summary>
    ///   The message of log entry. Example:
    ///   <code>Exception processing remote events from database: core</code>
    /// </summary>
    [NotNull]
    string Message { get; }

    /// <summary>
    ///   The raw stack trace property of Exception object. Example:
    ///   <code>
    /// Exception: System.Data.SqlClient.SqlException
    /// Message: A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: SQL Network Interfaces, error: 26 - Error Locating Server/Instance Specified)
    /// Source: .Net SqlClient Data Provider
    ///    at System.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
    ///    at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
    /// ...
    /// </code>
    /// </summary>
    ExceptionInfo Exception { get; }

    /// <summary>
    ///   The raw message only (without parsed date and time) of log entry. It may take one or several lines depending on
    ///   actual log format. Example:
    ///   <code>
    /// ManagedPoolThread #1 08:59:43 ERROR Exception processing remote events from database: core
    /// Exception: System.Data.SqlClient.SqlException
    /// Message: A network-related or instance-specific error occurred while establishing a connection to SQL Server. The server was not found or was not accessible. Verify that the instance name is correct and that SQL Server is configured to allow remote connections. (provider: SQL Network Interfaces, error: 26 - Error Locating Server/Instance Specified)
    /// Source: .Net SqlClient Data Provider
    ///    at System.Data.SqlClient.SqlInternalConnection.OnError(SqlException exception, Boolean breakConnection, Action`1 wrapCloseInAction)
    ///    at System.Data.SqlClient.TdsParser.ThrowExceptionAndWarning(TdsParserStateObject stateObj, Boolean callerHasConnectionLock, Boolean asyncClose)
    /// ...
    /// </code>
    /// </summary>
    [NotNull]
    string RawText { get; }
  }
}