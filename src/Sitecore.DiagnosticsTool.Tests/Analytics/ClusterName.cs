using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Diagnostics.Objects;
using Sitecore.DiagnosticsTool.Core.Categories;
using Sitecore.DiagnosticsTool.Core.Output;
using Sitecore.DiagnosticsTool.Core.Tests;

namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  public class ClusterName : Test
  {
    public override string Name { get; } = "Cluster name configuration";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt >= 75;
    }

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery);
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      var value = data.SitecoreInfo.GetSetting("Analytics.ClusterName", "default-cd-cluster");
      if (string.IsNullOrWhiteSpace(value) || value == "default-cd-cluster")
      {
        output.Error(new ShortMessage("The Analytics.ClusterName setting is not configured"), new Uri("https://sitecore.stackexchange.com/questions/4970/analytics-clustername-in-a-multi-site-scaled-environment"));
      }
    }
  }
}
