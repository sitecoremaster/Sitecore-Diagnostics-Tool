namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class RendererUrlSetting : KbTest
  {
    public override string KbName { get; } = "Customize the Renderer URL setting in ECM";

    public override string KbNumber => "837879";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentManagement);
    }

    [NotNull]
    protected string ErrorMessage => "The ECM.RendererUrl setting doesn't exist. This can cause issues with message dispatch in some environments.";

    protected override bool IsActual(ITestResourceContext data)
    {
      return data.SitecoreInfo.ModulesInformation.InstalledModules.ContainsKey("Email Experience Manager");
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var setting = data.SitecoreInfo.GetSetting("ECM.RendererUrl");
      if (string.IsNullOrEmpty(setting))
      {
        Report(output, ErrorMessage);
      }
    }
  }
}