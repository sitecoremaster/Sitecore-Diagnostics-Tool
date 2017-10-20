namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Schema
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public sealed class Column : Sitecore.Diagnostics.Database.Schema.Column
  {
    internal Column([NotNull] Diagnostics.SqlServer.Column column)
    {
      Assert.ArgumentNotNull(column, nameof(column));

      Type = column.DataType;
      Nullable = column.Nullable;
      Length = column.MaximumLength;
      PrimaryKey = column.InPrimaryKey;
      ForeignKey = column.IsForeignKey;
      Constraint = column.DefaultConstraint;
    }
  }
}