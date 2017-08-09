namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Database.Schema;

  /// <summary>
  ///   The read-only interface for accessing SQL database
  /// </summary>
  public interface ISqlDatabase : IDatabase
  {
    /// <summary>
    ///   The logical type of SQL database.
    /// </summary>
    DatabaseType Type { get; }

    /// <summary>
    ///   Represents the database schema.
    /// </summary>
    [NotNull]
    DatabaseSchema Schema { get; }

    [NotNull]
    IDatabaseMetrics Metrics { get; }

    /// <summary>
    ///   Represents a SQL query: SELECT COUNT(*) FROM tableName
    /// </summary>
    /// <param name="tableName">The table name.</param>
    /// <returns>The rows count.</returns>
    int CountRows([NotNull] string tableName);

    /// <summary>
    ///   Represents a SQL query: SELECT COUNT(*) FROM tableName WHERE condition
    /// </summary>
    /// <param name="tableName">The table name.</param>
    /// <param name="condition">The optional condition to filter rows with.</param>
    /// <returns>The rows count.</returns>
    int CountRows([NotNull] string tableName, string condition);
  }
}