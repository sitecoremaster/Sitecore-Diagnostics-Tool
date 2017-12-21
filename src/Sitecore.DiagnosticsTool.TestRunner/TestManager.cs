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
    private readonly List<ITest> GlobalTests = new List<ITest>();

    [NotNull]
    public IEnumerable<ITest> GetTests()
    {
      if (GlobalTests.Count > 0)
      {
        return GlobalTests;
      }

      var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
      if (path == null)
      {
        return GlobalTests;
      }

      var assemblyPaths = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories).Concat(Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories));
      foreach (var assemblyPath in assemblyPaths)
      {
        try
        {
          GlobalTests.AddRange(GetTests(assemblyPath));
        }
        catch (Exception exception)
        {
          Log.Error(exception, "Failed to load an assembly: " + assemblyPath);
        }
      }

      return GlobalTests;
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