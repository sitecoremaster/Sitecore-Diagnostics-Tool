namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System.Linq;

  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public class SolutionTestRunner : TestRunnerBase<ISolutionTest, ISolutionTestResourceContext, IDataProvider[]>
  {
    protected override ISolutionTestResourceContext CreateResoureContext(IDataProvider[] data)
    {
      return new SolutionTestResourceContext(data.Select(x => CreateContext(x)));
    }

    protected override bool IsTestActual(ISolutionTest test, ISolutionTestResourceContext data)
    {
      return test.IsActual(data, data.Values.FirstOrDefault().SitecoreInfo.SitecoreVersion);
    }

    protected override void ProcessTest(ISolutionTest test, ISolutionTestResourceContext data, ITestProcessingContext context)
    {
      test.Process(data, context);
    }
  }
}