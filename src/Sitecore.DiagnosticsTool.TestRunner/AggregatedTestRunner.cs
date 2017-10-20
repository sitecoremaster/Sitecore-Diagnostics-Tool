namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests;

  public static class AggregatedTestRunner
  {
    public static ResultsFile RunTests([NotNull] SupportPackageDataProvider[] packages, [CanBeNull] Action<ITestMetadata> onTestRun = null)
    {
      // get tests
      TestsLibrary.Init();
      var tests = new TestManager()
        .GetTests()
        .ToArray();

      var solutionTests = new SolutionTestManager()
        .GetTests()
        .ToArray();

      var testRunner = new TestRunner();

      // run simple tests
      var resultsFile = new ResultsFile();
      foreach (var package in packages)
      {
        var results = testRunner
          .RunTests(tests, package, onTestRun)
          .ToArray();

        resultsFile.Instances
          .Add(package.FileName, results);
      }

      // run solution tests
      resultsFile.Solution = new SolutionTestRunner()
        .RunTests(solutionTests, packages)
        .ToArray();

      resultsFile.Packages = packages.ToMap(x => x.FileName, x => x as IDataProvider);

      return resultsFile;
    }

    public static int GetTotalTestsCount()
    {
      return new TestManager().GetTests().Count() + new SolutionTestManager().GetTests().Count();
    }
  }
}