namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class CheckIndexFragmentation : Test
  {
    public override string Name { get; } = "SQL Server index fragmentation level check";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Performance };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      foreach (var database in data.Databases.Sql.All)
      {
        var arr = database.Metrics.Tables
          .SelectMany(t => t.Value.IndexesHealth
            .Select(i => new
            {
              Name = "Tables." + t.Key + ".Indexes." + i.Value.Name,
              Value = i.Value.AverageFragmentationInPercent
            }))
          .Where(x => x.Value > 10)
          .ToArray();

        if (arr.Length <= 0)
        {
          continue;
        }

        var indexes = arr.ToArray(x => $"{x.Name} ({(int)x.Value}%)");
        var message = new ShortMessage(
          new BoldText(database.Name),
          new Text($" database indexes require defragmentation."),
          new BulletedList(indexes),
          new Text("Refer to the 2.1 section in CMS Performance Tuning Guide for details."));

        output.Error(message);
      }
    }
  }
}