namespace Sitecore.DiagnosticsTool.Tests.General.Health
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class EventQueuePropertyChangedRemoteEvent : KbTest
  {
    private const string Condition = @"[InstanceType] LIKE 'Sitecore.Data.Eventing.Remote.PropertyChangedRemoteEvent%' AND [InstanceData] LIKE '%last_updated%'";

    protected const int RateLow = 30;

    protected const int RateMedium = 50;

    protected const int RateHigh = 75;

    public override string KbName { get; } = "Sitecore Content Search may cause performance issues due to excessive updates of the EventQueue table";

    public override string KbNumber => "930920";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.SearchIndexing, Category.Performance };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      var version = sitecoreVersion.MajorMinorInt;
      var update = sitecoreVersion.MajorMinorUpdateInt;
      if (version < 70 ||
        version == 72 && update >= 725 ||
        version == 80 && update >= 806 ||
        version == 81 && update >= 811 ||
        version >= 82)
      {
        return false;
      }

      return true;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var core = data.Databases.Sql["core"];
      if (core == null)
      {
        output.Error("The core database is not found");

        return;
      }

      var eventQueueSize = core.CountRows("EventQueue");
      if (eventQueueSize == 0)
      {
        return;
      }

      var propertyChangedEventsCount = core.CountRows("EventQueue", Condition);

      Assert.IsTrue(propertyChangedEventsCount >= 0, "propertyChangedEventsCount >= 0");
      Assert.IsTrue(eventQueueSize > 0, "eventQueueSize > 0");

      var ratio = propertyChangedEventsCount * 100 / eventQueueSize;
      if (ratio < RateLow)
      {
        return;
      }

      var message = $"The EventQueue table is more than {RateMedium}% filled with the PropertyChangedRemoteEvent records.";
      if (ratio < RateMedium)
      {
        output.Debug(message);
        return;
      }

      if (ratio < RateHigh)
      {
        output.Warning(message, Link);
        return;
      }

      output.Error(message, Link);
    }
  }
}