namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Schema
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public sealed class Table : Sitecore.Diagnostics.Database.Schema.Table
  {
    internal Table([NotNull] Diagnostics.SqlServer.Table table)
    {
      Assert.ArgumentNotNull(table, nameof(table));

      Columns = table.Columns.ToDictionary(x => x.Name, x => new Column(x) as Sitecore.Diagnostics.Database.Schema.Column);

      var indexes = new Dictionary<string, Sitecore.Diagnostics.Database.Schema.Index>();

      foreach (var index in table.Indexes)
      {
        try
        {
          indexes.Add(index.Name, new Index(index));
        }
        catch (Exception)
        {
          var smoIndexColumn = new IndexColumn(index.IndexedColumns[0]);
          indexes[index.Name].Columns.Add(index.IndexedColumns[0].Name, smoIndexColumn);
        }
      }

      Indexes = indexes;
    }
  }
}