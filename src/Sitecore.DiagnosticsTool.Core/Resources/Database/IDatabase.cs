namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using JetBrains.Annotations;

  public interface IDatabase
  {
    /// <summary>
    ///   The name of the appropriate connection string.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    ///   The appropriate connection string text.
    /// </summary>
    [NotNull]
    string ConnectionString { get; }
  }
}