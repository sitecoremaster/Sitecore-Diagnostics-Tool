namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class CpuCores : Test
  {
    // http://sdn.sitecore.net/upload/sitecore7/75/installation_guide_sc75-a4.pdf
    public override string Name { get; } = "Number of CPU cores";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Performance };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var coresCount = data.WebServer.Info.CpuCoresCount;

      var message = $"Your server has {coresCount} CPU cores, which is lower than the recommended minimum. Please review the hardware recommendations for Sitecore products.";
      if (coresCount <= 2)
      {
        output.Warning(message);
      }
    }
  }
}