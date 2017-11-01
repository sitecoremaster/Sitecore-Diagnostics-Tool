namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class DisableCacheSizeLimits : Test
  {
    public override string Name { get; } = "Cache size limits are disabled";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Performance};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (data.SitecoreInfo.GetBoolSetting("Caching.DisableCacheSizeLimits"))
      {
        output.Warning("All cache size limits are disabled. It can potentially cause significant performance degradation and OutOfMemoryException exception. It is recommended to disable this setting. Check CMS Performance Tuning Guide document for details.");
      }
    }
  }
}