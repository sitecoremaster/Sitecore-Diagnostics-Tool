namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using JetBrains.Annotations;

  /// <summary>
  ///   The read-only interface for accessing SQL databases
  /// </summary>
  public interface ISqlDatabaseContext
  {
    /// <summary>
    ///   The connection string names of all SQL databases.
    /// </summary>
    [NotNull]
    string[] DatabaseNames { get; }

    /// <summary>
    ///   The collection of read-only interfaces to all SQL databases.
    /// </summary>
    [NotNull]
    ISqlDatabase[] All { get; }

    /// <summary>
    ///   The read-only interface for accessing SQL database
    /// </summary>
    /// <param name="name">The name of the appropriate connection string.</param>
    ISqlDatabase this[string name] { get; }
  }
}