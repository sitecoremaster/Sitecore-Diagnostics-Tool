namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Diagnostics.SqlServer
{
  public class Column
  {
    public string Name { get; internal set; }

    public string DataType { get; set; }

    public bool Nullable { get; set; }

    public int MaximumLength { get; set; }

    public bool InPrimaryKey { get; set; }

    public bool IsForeignKey { get; set; }

    public string DefaultConstraint { get; set; }
  }
}