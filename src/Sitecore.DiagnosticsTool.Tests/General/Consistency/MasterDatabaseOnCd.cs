namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-07-31, fixed)
  [UsedImplicitly]
  public class MasterDatabaseOnCd : Test
  {
    protected const string SiteXPath = @"/configuration/sitecore/sites/site[@database='master']";

    protected const string IdTableXPath = @"/configuration/sitecore/IDTable/param[@connectionStringName='master']";

    protected const string DatabaseXPath = @"/configuration/sitecore/databases/database[@id='master']";

    protected const string SearchXPath = @"/configuration/sitecore/search/configuration/indexes/index/locations/master";

    protected const string SchedulingXPath1 = @"/configuration/sitecore/scheduling/agent/databases/database[@name='master']";

    protected const string SchedulingXPath2 = @"/configuration/sitecore/scheduling/agent/param";

    public override string Name { get; } = "Master database must not be referenced on CD instance";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      return roles.Contains(ServerRole.ContentDelivery);
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var config = data.SitecoreInfo.Configuration;

      var sites = config.SelectElements(SiteXPath);
      if (sites.Any())
      {
        output.Warning(GetErrorMessage("<sites>"));
      }

      var idTable = config.SelectElements(IdTableXPath);
      if (idTable.Any())
      {
        output.Warning(GetErrorMessage("<IDTable>"));
      }

      var databases = config.SelectElements(DatabaseXPath);
      if (databases.Any())
      {
        output.Warning(GetErrorMessage("<databases>"));
      }

      var search = config.SelectElements(SearchXPath);
      if (search.Any())
      {
        output.Warning(GetErrorMessage("<search>"));
      }

      var agent1 = config.SelectElements(SchedulingXPath1);
      if (agent1.Any())
      {
        output.Warning(GetErrorMessage("<scheduling>"));
      }

      var agent2 = config.SelectElements(SchedulingXPath2)
        .Where(x => x != null && x.InnerText == "master");

      if (agent2.Any())
      {
        output.Warning(GetErrorMessage("<scheduling>"));
      }
    }

    [NotNull]
    protected string GetErrorMessage([NotNull] string element)
    {
      Assert.ArgumentNotNull(element, nameof(element));

      return $"The master database is referenced in {element} section of Sitecore configuration.";
    }
  }
}