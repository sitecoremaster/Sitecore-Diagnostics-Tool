namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Schema
{
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public sealed class Index : Sitecore.Diagnostics.Database.Schema.Index
  {
    internal Index([NotNull] Diagnostics.SqlServer.Index index)
    {
      Assert.ArgumentNotNull(index, nameof(index));

      Columns = index.IndexedColumns.ToDictionary(x => x.Name, x => new IndexColumn(x) as Sitecore.Diagnostics.Database.Schema.IndexColumn);
      Type = index.IndexType;
      IgnoreDuplicateKeys = index.IgnoreDuplicateKeys;
      DisallowPageLocks = index.DisallowPageLocks;
      DisallowRowLocks = index.DisallowRowLocks;
      FileGroup = index.FileGroup;
      FillFactor = index.FillFactor;
      Filter = index.FilterDefinition;
      PadIndex = index.PadIndex;
    }
  }
}