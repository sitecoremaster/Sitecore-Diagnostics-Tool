namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class PublishingInstance : SolutionTest
  {
    protected const string PublishingInstanceSetting = "Publishing.PublishingInstance";

    public override string Name { get; } = "Publishing instance is set up consistently";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ISolutionTestResourceContext data, ITestOutputContext output)
    {
      var map = data

        // exclude all CD instances - they do not participate in publishing
        .Where(x => x.Value.ServerRoles.All(z => z != ServerRole.ContentDelivery))

        // group all publishing instance setting values to set { 'Pub1': ['Cm1', 'Cm2'], 'Pub2': ['BadCm3'], '': ['BadCm4'] }
        .GroupBy(x => x.Value.SitecoreInfo.GetSetting(PublishingInstanceSetting))
        .ToMap(x => x.Key, x => x.ToArray(z => z.Key));

      if (map.Count == 0)
      {
        output.Debug("No publishing instance specified");

        return;
      }

      if (map.Count == 1)
      {
        output.Debug($"Publishing instance is consistent: {map.Keys.FirstOrDefault().EmptyToNull() ?? "N/A"}");

        return;
      }

      var message = GetMessage(map);
      var detailed = GetDetailed(map);

      output.Error(message, detailed: detailed);
    }

    private static DetailedMessage GetDetailed(Map<string[]> map)
    {
      return new DetailedMessage(new Text("Publishing instance confiugration:"),
        new BulletedList(map.ToArray(x => new Container(
          new Text(x.Key.EmptyToNull() ?? "[empty]"),
          new Text(" is used as publishing instance by:"),
          new BulletedList(x.Value)))));
    }

    protected ShortMessage GetMessage([NotNull] Map<string[]> map)
    {
      // generate report message:
      //   Publishing instance configuration is inconsistent: there are 3 different values while only maximum 1 is supported. These values are: 
      //   - Pub1: Cm1, Cm2
      //   - Pub2: BadCm3
      //   - [empty]: BadCm4
      var message = new ShortMessage(
        new Text($"Publishing instance configuration is inconsistent: there are "), new BoldText($"{map.Count}"),
        new Text(" different values while only maximum "), new BoldText("1"), new Text(" is supported."));

      return message;
    }
  }
}