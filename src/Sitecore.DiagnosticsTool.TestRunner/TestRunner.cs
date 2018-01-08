namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests;

  public class TestRunner : TestRunnerBase<ITest, ISolutionResourceContext, IDataProvider[]>
  {
    protected override ISolutionResourceContext CreateResoureContext(IDataProvider[] data, ISystemContext system)
    {
      return new SolutionResourceContext(data.ToArray(CreateContext), system);
    }

    protected override bool IsTestActual(ITest test, ISolutionResourceContext data)
    {
      return test.IsActual(data, data.Values.FirstOrDefault().SitecoreInfo.SitecoreVersion);
    }

    protected override void ProcessTest(ITest test, ISolutionResourceContext data, ITestProcessingContext context)
    {
      test.Process(data, context);
    }

    public static ResultsFile RunTests([NotNull] SupportPackageDataProvider[] packages, [NotNull] ISystemContext system, [CanBeNull] Action<ITestMetadata, int, int> onTestRun = null)
    {
      // get tests
      TestsLibrary.Init();
      var solutionTests = new TestManager()
        .GetTests()
        .ToArray();

      var totalCount = solutionTests.Length;

      // run solution tests
      var resultsFile = new ResultsFile();
      resultsFile.Solution = new TestRunner()
        .RunTests(solutionTests, packages, system, (test, index) => onTestRun?.Invoke(test, index, totalCount))
        .ToArray();

      resultsFile.Packages = packages.ToMap(x => x.FileName, x => x as IDataProvider);

      return resultsFile;
    }
  }
}