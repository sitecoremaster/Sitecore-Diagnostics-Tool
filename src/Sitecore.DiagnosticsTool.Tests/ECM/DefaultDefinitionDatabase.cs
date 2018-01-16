namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

  public class DefaultDefinitionDatabase : Test
  {
    public override string Name => "Default Definition Database should be switched to 'web' on CD servers";

    public override IEnumerable<Category> Categories => new[] { Category.Ecm };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 8;
    }

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery);
    }
    protected override bool IsActual(IInstanceResourceContext data)
    {
      return data.SitecoreInfo.ModulesInformation.InstalledModules.ContainsKey("Email Experience Manager");
    }

    [NotNull]
    protected string ErrorMessage => "Analytics.DefaultDefinitionDatabase setting should be defined on CD server. 'web' should be used as a value.";

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (data.SitecoreInfo.GetSetting("Analytics.DefaultDefinitionDatabase") != "web")
      {
        output.Warning(ErrorMessage);
      }
    }
  }
}