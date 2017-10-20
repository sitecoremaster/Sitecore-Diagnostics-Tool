namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Diagnostics.SqlServer
{
  using System.Collections.Generic;

  public class Table
  {
    public string Name { get; set; }
    public List<Column> Columns { get; set; }
    public List<Index> Indexes { get; set; }
    public long RowCount { get; set; }
    public double DataSpaceUsed { get; set; }
  }
}