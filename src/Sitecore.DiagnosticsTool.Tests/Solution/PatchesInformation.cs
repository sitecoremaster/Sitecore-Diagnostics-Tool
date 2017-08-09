namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.DiagnosticsTool.Core.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class PatchesInformation : SolutionTest
  {
    public override string Name { get; } = "Patches Information";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ISolutionTestResourceContext data, ITestOutputContext output)
    {
      var patches = new Map<List<string>>();
      var instanceNames = data.Values.ToArray(x => x.SitecoreInfo.InstanceName);

      foreach (var instance in data.Values)
      {
        Safe(_ => ProcessAssemblies(instance, patches));
        Safe(_ => ProcessConfiguration(instance, patches));
      }

      var assembliesResult = PrintResults(patches, instanceNames);
      if (assembliesResult != null)
      {
        output.Debug(assembliesResult);
      }
    }

    private static void ProcessAssemblies(ITestResourceContext instance, Map<List<string>> assemblies)
    {
      var instanceName = instance.SitecoreInfo.InstanceName;
      foreach (var assembly in instance.SitecoreInfo.Assemblies.Values)
      {
        var assemblyName = Path.GetFileNameWithoutExtension(assembly.FileName);
        if (assemblyName.StartsWith("Sitecore.Support."))
        {
          var instanceNames = assemblies.GetOrAdd(assemblyName, new List<string>());
          if (!instanceNames.Contains(instanceName))
          {
            instanceNames.Add(instanceName);
          }
        }
      }
    }

    private static void ProcessConfiguration(ITestResourceContext instance, Map<List<string>> configs)
    {
      var instanceName = instance.SitecoreInfo.InstanceName;
      foreach (var filePath in instance.SitecoreInfo.IncludeFiles.Keys)
      {
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        if (!fileName.Contains("Sitecore.Support."))
        {
          continue;
        }

        var instanceNames = configs.GetOrAdd(fileName, new List<string>());
        if (!instanceNames.Contains(instanceName))
        {
          instanceNames.Add(instanceName);
        }
      }
    }

    private static DetailedMessage PrintResults([NotNull] Map<List<string>> patches, [NotNull] string[] instanceNames)
    {
      if (!patches.Any())
      {
        return null;
      }

      var rows = new List<TableRow>();
      foreach (var patchName in patches.Keys)
      {
        var cols = new List<Pair>
        {
          new Pair("Patch", patchName)
        };

        var assemblyInstances = patches[patchName];
        foreach (var instanceName in instanceNames)
        {
          cols.Add(new Pair(instanceName, assemblyInstances.Contains(instanceName) ? "Installed" : "No"));
        }

        rows.Add(new TableRow(cols));
      }

      return new DetailedMessage(new Text($"Solution contains {patches.Keys} patches installed"), new Table(rows.ToArray()));
    }

    private static void Safe(Action<Null> func)
    {
      try
      {
        func(null);
      }
      catch
      {
      }
    }
  }
}
