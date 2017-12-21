namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class AssembliesConsistencyMatch : Test
  {
    public override string Name { get; } = "Assemblies consistency check";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    protected const string VersionInconsistencyMessage = "Assembly version inconsistency detected";

    protected const string SizeInconsistencyMessage = "Assembly size inconsistency detected";

    protected const string AssemblyIsMissingMessage = "Assembly presence inconsistency detected";

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var actualAssemblies = data.SitecoreInfo.Assemblies;
      var defaultAssemblies = data.SitecoreInfo.SitecoreDefaults.Assemblies;

      var missing = new List<string>();
      var version = new List<string>();

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

        var key = actualAssemblies.Keys.FirstOrDefault(x => fileName.Equals(x, StringComparison.OrdinalIgnoreCase)) ?? actualAssemblies.Keys.FirstOrDefault(x => fileName.EndsWith(x, StringComparison.OrdinalIgnoreCase));
        if (key == null)
        {
          missing.Add(GetAssemblyIsMissingMessage(fileName));
          continue;
        }

        var actualAssembly = actualAssemblies[key];
        Assert.IsNotNull(actualAssembly, "assembly");

        var expectedVersion = defaultAssembly.FileVersion;
        if (!string.Equals(expectedVersion, actualAssembly.FileVersion, StringComparison.Ordinal))
        {
          version.Add(GetVersionInconsistencyMessage(actualAssembly, expectedVersion));

          continue;
        }

        var actualSize = actualAssembly.FileSize;
        if (actualSize == null)
        {
          output.CannotRun($"Cannot compare assembly file size as the data is not available: {actualAssembly.FileName}");

          continue;
        }

        // disabled because currently SIS returns invalid values for defaultAssembly.FileSize
        // var expectedSize = defaultAssembly.FileSize;
        // if (expectedSize != actualSize)
        // {
        //   output.Warning(GetSizeInconsistencyMessage(actualAssembly, expectedSize));
        // }
      }

      if (missing.Any())
      {
        missing.Sort();
        output.Error(AssemblyIsMissingMessage, null, new DetailedMessage(new BulletedList(missing)));
      }

      if (version.Any())
      {
        version.Sort();
        output.Warning(VersionInconsistencyMessage, null, new DetailedMessage(new BulletedList(version)));
      }
    }

    [NotNull]
    protected string GetVersionInconsistencyMessage([NotNull] AssemblyFile assembly, [NotNull] string formalVersion)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));
      Assert.ArgumentNotNull(formalVersion, nameof(formalVersion));

      return $"{assembly.FileName} (actual: {assembly.FileVersion}, expected: {formalVersion}).";
    }

    [NotNull]
    protected string GetSizeInconsistencyMessage([NotNull] AssemblyFile assembly, long? formalSize)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));
      Assert.ArgumentNotNull(formalSize, nameof(formalSize));

      return $"{assembly.FileName} (actual: {assembly.FileSize} bytes, expected: {formalSize} bytes).";
    }

    [NotNull]
    protected string GetAssemblyIsMissingMessage([NotNull] string fileName)
    {
      Assert.ArgumentNotNull(fileName, nameof(fileName));

      return $"{fileName.Trim()} (assembly is missing).";
    }
  }
}