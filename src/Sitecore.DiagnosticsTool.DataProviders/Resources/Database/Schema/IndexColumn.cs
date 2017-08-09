namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Schema
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Diagnostics.SqlServer;

  public sealed class IndexColumn : Sitecore.Diagnostics.Database.Schema.IndexColumn
  {
    public IndexColumn([NotNull] IndexedColumn indexedColumn)
    {
      Assert.ArgumentNotNull(indexedColumn, nameof(indexedColumn));

      IsIncluded = indexedColumn.IsIncluded;
      IsComputed = indexedColumn.IsComputed;
      Descending = indexedColumn.Descending;
    }
  }
}