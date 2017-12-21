namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   The read-only interface for accessing Sitecore databases
  /// </summary>
  public interface IDatabaseContext : IResource
  {
    /// <summary>
    ///   The read-only interface for accessing SQL databases
    /// </summary>
    [NotNull]
    ISqlDatabaseContext Sql { get; }

    /// <summary>
    ///   The read-only interface for accessing Mongo databases
    /// </summary>
    [NotNull]
    IMongoDatabaseContext Mongo { get; }

    // TODO: Rework to have own wrapper to support searching case-insensitive 
    /// <summary>
    ///   The read-only interface for accessing plain connection strings.
    /// </summary>
    [NotNull]
    IReadOnlyDictionary<string, string> ConnectionStrings { get; }
  }
}