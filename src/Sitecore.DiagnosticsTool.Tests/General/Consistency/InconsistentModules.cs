namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System;

  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.Objects;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class InconsistentModules : Test
  {
    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override string Name { get; } = "Inconsistent Modules";

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      
      foreach (var item in data.SitecoreInfo.ModulesInformation.IncorrectlyInstalledModules)
      {
        if (!item.Value.Any())
        {
          continue;
        }

        var version = item.Value.Select(
            ri =>
              new
              {
                ri.Release,
                Data =
                ri.Assemblies
                  .Select(x => new
                  {
                    x.FileName,
                    Expected = x.FileVersion,
                    Actual = data.SitecoreInfo.Assemblies.TryGetValue(x.FileName)?.FileVersion ?? "[missing]"
                  })
                  .Where(x => x.Actual != x.Expected)
              })

          .Select(ri => new
          {
            ri.Release,
            ri.Data,
            Count = ri.Data.Count(),
          })

          .Where(z => z.Data.Count(x => x.Actual != "[missing]" && x.FileName.StartsWith("Sitecore.", StringComparison.OrdinalIgnoreCase)) > 0) // with at least one existing Sitecore assembly in the list
          .OrderBy(z => z.Count)
          .FirstOrDefault();

        if (version == null)
        {
          continue;
        }
          
        output.Debug(
          new DetailedMessage(new Text($"An inconsistent module was detected: "), 
          new BoldText(item.Key + " " + version.Release.Version.MajorMinorUpdate),
          new Table(
            version.Data
              .ToArray(x => 
                new TableRow(
                  new Pair("Assembly", x.FileName), 
                  new Pair("Actual Version", x.Actual), 
                  new Pair("Default Version", x.Expected))))));
      }
    }
  }
}