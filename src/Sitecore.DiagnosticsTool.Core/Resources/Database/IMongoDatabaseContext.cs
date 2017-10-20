namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using JetBrains.Annotations;

  /// <summary>
  ///   The read-only interface for accessing Mongo databases
  /// </summary>
  public interface IMongoDatabaseContext
  {
    /// <summary>
    ///   The connection string names of all mongo databases.
    /// </summary>
    [NotNull]
    string[] DatabaseNames { get; }

    /// <summary>
    ///   The collection of read-only interfaces to all Mongo databases.
    /// </summary>
    [NotNull]
    IMongoDatabase[] All { get; }

    /// <summary>
    ///   The read-only interface for accessing Mongo database
    /// </summary>
    /// <param name="name">The name of the appropriate connection string.</param>
    IMongoDatabase this[string name] { get; }
  }
}