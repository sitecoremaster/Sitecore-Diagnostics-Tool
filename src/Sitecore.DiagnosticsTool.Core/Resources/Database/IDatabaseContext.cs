namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
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
  }
}