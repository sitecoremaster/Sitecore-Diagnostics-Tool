namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class AssembliesConsistencyMatch : SolutionTest
  {
    public override string Name { get; } = "Assemblies consistency check";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ISolutionResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var defaultAssemblies = data.SitecoreDefaults.Assemblies;

      var resultsMap = new Map<Map>();

      foreach (var defaultAssembly in defaultAssemblies.Values)
      {
        if (defaultAssembly == null)
        {
          continue;
        }

        var fileName = defaultAssembly.FileName;

        // workaround for sspg-49
        if (fileName.Equals("ninject.dll", StringComparison.OrdinalIgnoreCase))
        {
          continue;
        }

        var assemblyMap = new Map();

        var expectedFileVersion = defaultAssembly.FileVersion;
        assemblyMap.Add("Version", expectedFileVersion);

        var good = true;
        foreach (var instance in data.Values)
        {
          var result = Process(fileName, instance, defaultAssembly);
          if (string.IsNullOrEmpty(result))
          {
            assemblyMap.Add(instance.InstanceName, "OK");
          }
          else
          {
            assemblyMap.Add(instance.InstanceName, result);
            good = false;
          }
        }

        if (!good)
        {
          resultsMap.Add(fileName, assemblyMap);
        }
      }

      if (resultsMap.Count > 0)
      {
        output.Warning("There is an inconsistency in assembly files",
          detailed: new DetailedMessage(new Table(resultsMap.ToArray(x =>
            new TableRow(
              new[] { new Pair("Assembly", x.Key) }
                .Concat(x.Value.Select(c => new Pair(c.Key, c.Value)))
                .ToArray())))));
      }
    }

    private string Process(string fileName, IInstanceResourceContext instance, AssemblyFile defaultAssembly)
    {
      var actualAssemblies = instance.SitecoreInfo.Assemblies;
      var key = actualAssemblies.Keys.FirstOrDefault(x => fileName.Equals(x, StringComparison.OrdinalIgnoreCase)) ?? actualAssemblies.Keys.FirstOrDefault(x => fileName.EndsWith(x, StringComparison.OrdinalIgnoreCase));
      if (key == null)
      {
        return "missing";
      }

      var actualAssembly = actualAssemblies[key];
      Assert.IsNotNull(actualAssembly, "assembly");

      var actualFileVersion = actualAssembly.FileVersion;
      if (!string.Equals(defaultAssembly.FileVersion, actualFileVersion, StringComparison.Ordinal))
      {
        return actualFileVersion;
      }

      var actualHash = actualAssembly.HashSum;
      if (defaultAssembly.HashSum != actualHash)
      {
        return "[wrong checksum]";
      }

      return null;
    }
  }
}