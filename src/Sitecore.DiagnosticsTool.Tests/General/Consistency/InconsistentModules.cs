namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class InconsistentModules : Test
  {
    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override string Name { get; } = "Inconsistent Modules";

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var unique = new List<string>();

      // TODO: Remake
      foreach (var item in data.SitecoreInfo.ModulesInformation.IncorrectlyInstalledModules)
      {
        if (!unique.Contains(item.Key))
        {
          output.Debug($"An inconsistent module was detected: {item.Key}.");
          unique.Add(item.Key);
        }
      }
    }
  }
}