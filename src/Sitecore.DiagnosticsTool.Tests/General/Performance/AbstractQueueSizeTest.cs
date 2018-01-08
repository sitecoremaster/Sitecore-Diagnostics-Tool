namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public abstract class AbstractQueueSizeTest : Test
  {
    protected abstract int Recommended { get; }

    [NotNull]
    protected abstract string TableName { get; }

    public override string Name => TableName + " table size";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Performance };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (!TableName.Equals("EventQueue", StringComparison.InvariantCultureIgnoreCase))
      {
        return;
      }

      var databases = data.Databases.Sql.All;
      foreach (var database in databases)
      {
        if (database == null || database.Type != DatabaseType.Core && database.Type != DatabaseType.Content)
        {
          continue;
        }

        var count = database.CountRows(TableName);
        if (count < 0)
        {
          output.CannotRun($"Cannot check the {database.Name} database (value: -1)");
        }
        else if (count >= Recommended)
        {
          output.Warning(GetErrorMessage(database.Name, count));
        }
      }
    }

    [NotNull]
    protected string GetErrorMessage([NotNull] string databaseName, int count)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));

      return $"The {TableName} table of the {databaseName} database contains {count} rows which exceeds recommended maximum. Read more in CMS Performance Tuning Guide, section Cleanup Database Tables";
    }
  }
}