using Sitecore.Diagnostics.FileSystem;

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

      if (tests.Count == 0)
      {
        var dir = new FileSystem().ParseFile(Assembly.GetExecutingAssembly().Location).Directory;
        if (dir.Exists)
        {
          var assemblies = Enumerable.Concat(
            dir.GetFiles("*.dll", SearchOption.AllDirectories), 
            dir.GetFiles("*.exe", SearchOption.AllDirectories));

          foreach (var assembly in assemblies)
          {
            try
            {
              tests.AddRange(GetTests(Assembly.LoadFrom(assembly.FullName)));
            }
            catch (Exception exception)
            {
              Log.Error(exception, "Failed to load assembly: " + assembly.FullName);
            }
          }
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