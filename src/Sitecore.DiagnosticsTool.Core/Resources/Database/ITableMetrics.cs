namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using System.Collections.Generic;

  public interface ITableMetrics
  {
    string Name { get; }

    long RowCount { get; }

    double PhysicalSize { get; }

    Dictionary<string, IIndexHealthMetrics> IndexesHealth { get; }
  }
}