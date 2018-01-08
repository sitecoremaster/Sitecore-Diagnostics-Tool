namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;

  public class SolutionUnitTestContext : ISolutionUnitTestContext
  {
    [NotNull]
    private List<IDataProvider> Instances { get; } = new List<IDataProvider>();

    public ISolutionUnitTestContext AddInstance(IDataProvider dataProvider)
    {
      Instances.Add(dataProvider);

      return this;
    }

    public ISolutionUnitTestResultsContext Process(ITest test)
    {
      Assert.ArgumentNotNull(test, nameof(test));

      return new SolutionUnitTestResultsContext(test, Instances);
    }
  }
}