namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests;

  public static class AggregatedTestRunner
  {
    public static ResultsFile RunTests([NotNull] SupportPackageDataProvider[] packages, [NotNull] ISystemContext system, [CanBeNull] Action<ITestMetadata, int, int> onTestRun = null)
    {
      // get tests
      TestsLibrary.Init();
      var tests = new TestManager()
        .GetTests()
        .ToArray();

      var solutionTests = new SolutionTestManager()
        .GetTests()
        .ToArray();

      var totalCount = GetTotalTestsCount(packages.Length);
      var testRunner = new TestRunner();

      // run simple tests
      var resultsFile = new ResultsFile();
      for (var i = 0; i < packages.Length; i++)
      {
        var package = packages[i];
        var results = testRunner
          .RunTests(tests, package, system, (test, index) => onTestRun?.Invoke(test, index + tests.Length * i, totalCount))
          .ToArray();

        resultsFile.Instances
          .Add(package.FileName, results);
      }

      // run solution tests
      resultsFile.Solution = new SolutionTestRunner()
        .RunTests(solutionTests, packages, system, (test, index) => onTestRun?.Invoke(test, index + tests.Length * packages.Length, totalCount))
        .ToArray();

      resultsFile.Packages = packages.ToMap(x => x.FileName, x => x as IDataProvider);

      return resultsFile;
    }

    public static int GetTotalTestsCount(int packagesCount = 1)
    {
      return new TestManager().GetTests().Count() * packagesCount + new SolutionTestManager().GetTests().Count();
    }
  }
}