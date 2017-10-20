namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class IisVersionTest : Test
  {
    public override string Name { get; } = "IIS version for Sitecore XP";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major == 8;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (data.WebServer.Info.IisVersion.ProductMajorPart < 7)
      {
        output.Error("IIS version is earlier one than officially supported. Sitecore XP supports IIS versions: 7.0, 7.5, 8.0, 8.5");
      }
    }
  }
}