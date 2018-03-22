namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class ConsistentConfiguration : Test
  {
    protected const string ShortMessage = "One or several Sitecore instances in the solution have different sets of configuration files in App_Config folder's subfolders. It is recommended to have them idential across all Sitecore instances utilizing Configuration Rules engine";

    public override string Name { get; } = "Consistent configuration";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major == 9;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      var missing = new List<string>();
      var different = new List<string>();
      var defaults = data.SitecoreInfo.SitecoreDefaults.ConfigurationFiles.Values;
      var actual = data.SitecoreInfo.ConfigurationFiles.Values.ToArray();
      var ignorePaths = new[] { "App_Config\\ConnectionStrings.config" };
      foreach (var defaultFile in defaults)
      {
        var defaultPath = defaultFile.FilePath.Substring(defaultFile.FilePath.LastIndexOf("App_Config"));
        if (ignorePaths.Any(x => x.Equals(defaultPath, StringComparison.OrdinalIgnoreCase)))
        {
          continue;
        }

        var actualFile = actual.FirstOrDefault(x => x.FilePath.IndexOf(defaultPath, StringComparison.OrdinalIgnoreCase) >= 0);
        if (actualFile == null)
        {
          missing.Add(defaultPath);
          continue;
        }

        var defaultValue = defaultFile.RawText;

        // TODO: improve comparison
        if (actualFile.RawText != defaultValue)
        {
          different.Add(defaultPath);
        }
      }

      if (missing.Any())
      {
        output.Warning("One or several default configuration files are missing which may lead to unpredicted behavior", null, new DetailedMessage(new BulletedList(missing)));
      }

      if (different.Any())
      {
        output.Warning("The contents of one or several configuration files may have been changed which may cause troubleshooting or upgrade issues", null, new DetailedMessage(new BulletedList(different)));
      }
    }
  }
}