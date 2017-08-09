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

  public class TestManagerBase<TTest>
  {
    private readonly List<TTest> GlobalTests = new List<TTest>();

    [NotNull]
    public IEnumerable<TTest> GetTests()
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

      var assemblyPaths = Enumerable.Concat(Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories), Directory.GetFiles(path, "*.exe", SearchOption.AllDirectories));
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
    private IEnumerable<TTest> GetTests([NotNull] string assemblyPath)
    {
      Assert.ArgumentNotNull(assemblyPath, nameof(assemblyPath));
      Assert.ArgumentCondition(File.Exists(assemblyPath), nameof(assemblyPath), $"The {assemblyPath} file does not exist");

      var assembly = Assembly.LoadFrom(assemblyPath);
      var tests = GetTests(assembly);

      return tests;
    }

    [NotNull]
    private IEnumerable<TTest> GetTests([NotNull] Assembly assembly)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));

      var types = assembly.GetTypes();
      foreach (var type in types.Where(type => type.GetInterfaces().Contains(typeof(TTest)) && !type.IsInterface && !type.IsAbstract))
      {
        yield return (TTest)Activator.CreateInstance(type);
      }
    }
  }
}