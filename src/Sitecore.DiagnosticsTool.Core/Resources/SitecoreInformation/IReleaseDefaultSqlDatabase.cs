namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Database.Schema;

  public interface IReleaseDefaultSqlDatabase
  {
    /// <summary>
    ///   Logical name of the database.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    ///   Represents database schema.
    /// </summary>
    [NotNull]
    DatabaseSchema Schema { get; }
  }
}