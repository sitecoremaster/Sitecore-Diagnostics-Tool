namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class TestManager
  {
    [NotNull]
    public IEnumerable<ITest> GetTests()
    {
      var tests = new List<ITest>();
      foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        try
        {
          tests.AddRange(GetTests(assembly));
        }
        catch (Exception exception)
        {
          Log.Error(exception, "Failed to parse tests in already loaded assembly: " + assembly.FullName);
        }
      }

      return tests;
    }

    [NotNull]
    private IEnumerable<ITest> GetTests([NotNull] string assemblyPath)
    {
      Assert.ArgumentNotNull(assemblyPath, nameof(assemblyPath));
      Assert.ArgumentCondition(File.Exists(assemblyPath), nameof(assemblyPath), $"The {assemblyPath} file does not exist");

      var assembly = Assembly.LoadFrom(assemblyPath);
      var tests = GetTests(assembly);

      return tests;
    }

    [NotNull]
    private IEnumerable<ITest> GetTests([NotNull] Assembly assembly)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));

      var types = assembly.GetTypes();
      foreach (var type in types.Where(type => type.GetInterfaces().Contains(typeof(ITest)) && !type.IsInterface && !type.IsAbstract))
      {
        yield return (ITest)Activator.CreateInstance(type);
      }
    }
  }
}