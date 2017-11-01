namespace Sitecore.DiagnosticsTool.Tests.General.Consistency
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class AssembliesConsistencyMatch : Test
  {
    public override string Name { get; } = "Assemblies consistency check";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var actualAssemblies = data.SitecoreInfo.Assemblies;
      var defaultAssemblies = data.SitecoreInfo.SitecoreDefaults.Assemblies;

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
          output.Error(GetAssemblyIsMissingMessage(fileName));
          continue;
        }

        var actualAssembly = actualAssemblies[key];
        Assert.IsNotNull(actualAssembly, "assembly");

        var expectedVersion = defaultAssembly.FileVersion;
        if (!string.Equals(expectedVersion, actualAssembly.FileVersion, StringComparison.Ordinal))
        {
          output.Warning(GetVersionInconsistencyMessage(actualAssembly, expectedVersion));

          continue;
        }

        var actualSize = actualAssembly.FileSize;
        if (actualSize == null)
        {
          output.CannotRun($"Cannot compare assembly file size as the data is not available: {actualAssembly.FileName}");

          continue;
        }

        var expectedSize = defaultAssembly.FileSize;
        if (expectedSize != actualSize)
        {
          output.Warning(GetSizeInconsistencyMessage(actualAssembly, expectedSize));
        }
      }
    }

    [NotNull]
    protected string GetVersionInconsistencyMessage([NotNull] AssemblyFile assembly, [NotNull] string formalVersion)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));
      Assert.ArgumentNotNull(formalVersion, nameof(formalVersion));

      return $"Assembly version inconsistency detected for {assembly.FileName} (actual: {assembly.FileVersion}, expected: {formalVersion}).";
    }

    [NotNull]
    protected string GetSizeInconsistencyMessage([NotNull] AssemblyFile assembly, long? formalSize)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));
      Assert.ArgumentNotNull(formalSize, nameof(formalSize));

      return $"Assembly size inconsistency detected for {assembly.FileName} (actual: {assembly.FileSize} bytes, expected: {formalSize} bytes).";
    }

    [NotNull]
    protected string GetAssemblyIsMissingMessage([NotNull] string fileName)
    {
      Assert.ArgumentNotNull(fileName, nameof(fileName));

      return $"Assembly presence inconsistency detected for {fileName.Trim()} (assembly is missing).";
    }
  }
}