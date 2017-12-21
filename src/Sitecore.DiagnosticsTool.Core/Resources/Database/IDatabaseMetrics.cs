namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  public interface IDatabaseMetrics
  {
    [NotNull]
    [PublicAPI]
    string Name { get; }

    [NotNull]
    [PublicAPI]
    Dictionary<string, ITableMetrics> Tables { get; }
  }
}