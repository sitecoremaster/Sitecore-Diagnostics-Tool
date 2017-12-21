namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class DotNetFrameworkVersion : Test
  {
    public override string Name { get; } = ".NET Framework version";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major == 8
        ;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (data.WebServer.Info.FrameworkVersions.All(p => !FrameworkVersion.v45x.HasFlag(p)))
      {
        output.Error("Sitecore XP requires .NET Framework 4.5.");
      }
    }
  }
}