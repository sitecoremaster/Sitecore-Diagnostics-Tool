namespace Sitecore.DiagnosticsTool.TestRunner
{
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public class TestRunner : TestRunnerBase<ILegacyTest, ITestResourceContext, IDataProvider>
  {
    protected override bool IsTestActual(ILegacyTest test, ITestResourceContext data)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(data, nameof(data));

      return test.IsActual(data.ServerRoles, data.SitecoreInfo.SitecoreVersion, data);
    }

    protected override void ProcessTest(ILegacyTest test, ITestResourceContext data, ITestProcessingContext context)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(context, nameof(context));

      test.Process(data, context);
    }

    protected override ITestResourceContext CreateResoureContext(IDataProvider data, ISystemContext system)
    {
      return CreateContext(data);
    }
  }
}