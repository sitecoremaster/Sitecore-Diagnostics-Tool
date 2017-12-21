namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class GlobalAsaxInheritsFrom : SolutionTest
  {
    protected const string SystemWebMessage = "The Global.asax file must use the Sitecore.Web.Application base class or inherit from it.";
    protected const string Comment = "The given Global.asax file contains a reference to the custom application class, therefore manual verification is required.";

    public override string Name { get; } = "Global.asax should use Sitecore.Web.Application";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.MajorMinorInt >= 65;
    }

    public override void Process(ISolutionResourceContext solution, ITestOutputContext output)
    {
      var instancesWithWrongGlobalAsax = new List<string>();
      var instancesWithUnclear = new Map();
      foreach (var data in solution.Values)
      {
        var originalGlobalAsaxFile = data.SitecoreInfo.GlobalAsaxFile;
        var globalAsaxFile = originalGlobalAsaxFile.Replace(" ", string.Empty).ToLower().Replace("'", "\"");

        // If inherits points to System.Web.HttpApplication, output an error. 
        if (globalAsaxFile.Contains("Inherits=\"System.Web.HttpApplication\"".ToLower()))
        {
          instancesWithWrongGlobalAsax.Add(data.InstanceName);

          continue;
        }

        // If inherits is missing, output an error. 
        if (!globalAsaxFile.Contains("Inherits".ToLower()))
        {
          instancesWithWrongGlobalAsax.Add(data.InstanceName);

          continue;
        }

        // If inherits points to something else, output a warning that user needs to check manually if class inherits from Sitecore.Web.Application.
        if (!(globalAsaxFile.Contains("Inherits=\"Sitecore.Web.Application\"".ToLower())
          || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.CastleWindsorIntegration.WindsorApplication\"".ToLower())
          || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.AutoFacIntegration.AutoFacApplication\"".ToLower())
          || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.NinjectIntegration.NinjectApplication\"".ToLower())
          || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.StructureMapIntegration.StructureMapApplication\"".ToLower())
          || globalAsaxFile.Contains("Inherits=\"Sitecore.ContentSearch.SolrProvider.UnityIntegration.UnityApplication\"".ToLower())))
        {
          instancesWithUnclear.Add(data.InstanceName, originalGlobalAsaxFile);          
        }
      }

      if (instancesWithWrongGlobalAsax.Count > 0)
      {
        output.Error(SystemWebMessage, detailed: new DetailedMessage(new BulletedList(instancesWithWrongGlobalAsax)));
      }

      if (instancesWithUnclear.Count > 0)
      {
        output.Warning(
          new ShortMessage(
            new Text(SystemWebMessage),
            new Text(Comment)),
          detailed: new DetailedMessage(new Table(instancesWithUnclear.ToArray(x => new TableRow(new Pair("Instance", x.Key), new Pair("Value", x.Value))))));
      }
    }
  }
}