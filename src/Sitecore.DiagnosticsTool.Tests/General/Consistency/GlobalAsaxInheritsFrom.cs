namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class GlobalAsaxInheritsFrom : Test
  {
    protected const string SystemWebMessage = "The Global.asax file must use the Sitecore.Web.Application base class or inherit from it.";

    public override string Name { get; } = "Global.asax should use Sitecore.Web.Application";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt >= 65;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var globalAsaxFile = data.SitecoreInfo.GlobalAsaxFile.Replace(" ", string.Empty).ToLower().Replace("'", "\"");

      // If inherits points to System.Web.HttpApplication, output an error. 
      if (globalAsaxFile.Contains("Inherits=\"System.Web.HttpApplication\"".ToLower()))
      {
        output.Error(SystemWebMessage);
        return;
      }

      // If inherits is missing, output an error. 
      if (!globalAsaxFile.Contains("Inherits".ToLower()))
      {
        output.Error(SystemWebMessage);
        return;
      }

      // If inherits points to something else, output a warning that user needs to check manually if class inherits from Sitecore.Web.Application.
      if (!(globalAsaxFile.Contains("Inherits=\"Sitecore.Web.Application\"".ToLower())
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.CastleWindsorIntegration.WindsorApplication\"".ToLower())
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.AutoFacIntegration.AutoFacApplication\"".ToLower())
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.NinjectIntegration.NinjectApplication\"".ToLower())
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.StructureMapIntegration.StructureMapApplication\"".ToLower())
        || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.UnityIntegration.UnityApplication\"".ToLower())))
      {
        output.Warning(SystemWebMessage + " Actual value is " + globalAsaxFile);
      }
    }
  }
}