namespace Sitecore.DiagnosticsTool.Tests.Debug
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ChangedSettings : SolutionTest
  {
    [UsedImplicitly]
    public ChangedSettings()
    {
    }

    public override string Name { get; } = "Changed Settings";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ISolutionResourceContext solution, ITestOutputContext output)
    {
      var rows = new List<TableRow>();
      foreach (var settingName in solution.SitecoreDefaults.GetSettings().Keys)
      {
        var defaultValue = solution.SitecoreDefaults.GetSetting(settingName);

        if (solution.Values.Any(x => x.SitecoreInfo.GetSetting(settingName) != defaultValue))
        {
          var columns = new List<Pair> 
          { 
            new Pair("Setting", settingName),
            new Pair("Default Value", defaultValue.EmptyToNull() ?? "[empty]")            
          };
          columns.AddRange(solution.Values.Select(x => new Pair(x.InstanceName, x.SitecoreInfo.GetSetting(settingName))));

          rows.Add(new TableRow(columns));
        }
        
      }

      if (rows.Any())
      {
        output.Debug(new DetailedMessage(new Text("One or several default Sitecore settings were modified:"), new Table(rows.ToArray())));
      }
    }
  }
}