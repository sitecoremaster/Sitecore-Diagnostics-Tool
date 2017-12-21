namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class SchedulingSectionOnCd : Test
  {
    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };
    
    public override string Name { get; } = "EXM scheduling section must be disabled on ContentDelivery instance";

    [NotNull]
    protected string ErrorMessage => "EXM agents found in scheduling section. These agents should be disabled on ContentDelivery instance.";

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery) && !roles.Contains(ServerRole.ContentManagement);
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (
        data.SitecoreInfo.Configuration.SelectSingleNode("/configuration/sitecore/scheduling/agent[@hint='ECM']") != null ||
        data.SitecoreInfo.Configuration.SelectSingleNode("/configuration/sitecore/scheduling/agent[@hint='EXM tasks']") != null ||
        data.SitecoreInfo.Configuration.SelectSingleNode("/configuration/sitecore/scheduling/agent[@hint='EXM instance task']") != null)
      {
        output.Error(ErrorMessage);
      }
    }
  }
}