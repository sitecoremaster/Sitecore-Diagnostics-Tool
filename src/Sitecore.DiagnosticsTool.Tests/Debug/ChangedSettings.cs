namespace Sitecore.DiagnosticsTool.Tests.Debug
{
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ChangedSettings : Test
  {
    public override string Name { get; } = "Changed Settings";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var rows = new List<TableRow>();
      foreach (var settingName in data.SitecoreInfo.SitecoreDefaults.GetSettings().Keys)
      {
        var actualValue = data.SitecoreInfo.GetSetting(settingName);
        var defaultValue = data.SitecoreInfo.SitecoreDefaults.GetSetting(settingName);
        if (actualValue != defaultValue)
        {
          rows.Add(new TableRow(new Pair("Name", settingName), new Pair("Value", actualValue), new Pair("Default", defaultValue.EmptyToNull() ?? "[empty]")));
        }
      }

      if (rows.Any())
      {
        output.Debug(new DetailedMessage(new Text("One or several default Sitecore settings were modified:"), new Table(rows.ToArray())));
      }
    }
  }
}