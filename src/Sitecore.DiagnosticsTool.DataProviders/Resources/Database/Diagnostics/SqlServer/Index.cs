namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Diagnostics.SqlServer
{
  using System.Collections.Generic;

  public class Index
  {
    public Index()
    {
      IndexedColumns = new List<IndexedColumn>();
    }

    public string Name { get; set; }
    public string IndexType { get; set; }
    public bool IgnoreDuplicateKeys { get; set; }
    public bool DisallowPageLocks { get; set; }
    public bool DisallowRowLocks { get; set; }
    public string FileGroup { get; set; }
    public int FillFactor { get; set; }
    public string FilterDefinition { get; set; }
    public bool PadIndex { get; set; }
    public List<IndexedColumn> IndexedColumns { get; set; }
    public double AverageFragmentationInPercent { get; set; }
    public long PageCount { get; set; }
  }
}