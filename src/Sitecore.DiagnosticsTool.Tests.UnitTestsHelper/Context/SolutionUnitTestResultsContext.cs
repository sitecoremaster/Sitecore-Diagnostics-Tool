namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public class SolutionUnitTestResultsContext : ISolutionUnitTestResultsContext
  {
    [NotNull]
    private IList<ITestResult> Results { get; }

    public SolutionUnitTestResultsContext([NotNull] ITest test, IReadOnlyList<IDataProvider> resources)
    {
      Assert.ArgumentNotNull(test, nameof(test));

      var runner = new TestRunnerEx();
      var context = resources.ToArray();

      ITestReport report = null;
      if (test != null)
      {
        try
        {
          report = runner.RunTests(new[] { test }, context, new SystemContext("unit tests")).Single();
        }
        catch (ResourceNotAvailableException)
        {
        }
      }

      if (report == null)
      {
        report = runner.RunTests(new[] { test }, context, new SystemContext("unit tests")).Single();
      }

      // run test
      Results = report.Results.All.ToList();
    }

    public ISolutionUnitTestResultsContext MustReturn(ITestResult testResult, ComparisonMode mode)
    {
      Assert.ArgumentNotNull(testResult, nameof(testResult));

      var list = Results;
      var result = list.FirstOrDefault(x => testResult.State == x.State && (mode == ComparisonMode.Strict ? testResult.Message.ToString() == x.Message.ToString() : x.Message.ToString().StartsWith(testResult.Message.ToString())));
      if (result == null || (mode == ComparisonMode.StartsWith || result.Detailed != testResult.Detailed))
      {
        if (list.Any())
        {
          throw new InvalidOperationException($"The test didn't return expected test result, another results were returned instead.\r\n\r\nExpected:{new BulletedList(testResult.ToString()).ToString().EmptyToNull() ?? "[EMPTY]"}\r\nActual:{new BulletedList(list).ToString().EmptyToNull() ?? "[EMPTY]"}");
        }
        else
        {
          throw new InvalidOperationException($"The test didn't return expected test result:{new BulletedList(testResult.ToString()).ToString().EmptyToNull() ?? "[EMPTY]"}");
        }
      }
      else
      {
        list.Remove(result);
      }

      return this;
    }

    public void Done()
    {
      var list = Results;
      var count = list.Count;
      if (count > 0)
      {
        throw new InvalidOperationException($"There are {count} more test results than expected:{new BulletedList(list)}");
      }
    }

    public class TestRunnerEx : TestRunner
    {
      protected override bool UnitTesting { get; } = true;
    }
  }
}