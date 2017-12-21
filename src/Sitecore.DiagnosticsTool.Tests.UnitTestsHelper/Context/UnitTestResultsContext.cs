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
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  public class UnitTestResultsContext : IUnitTestResultsContext
  {
    [NotNull]
    private IList<ITestResult> Results { get; }

    public UnitTestResultsContext([NotNull] ILegacyTest test, IReadOnlyList<IResource> resources)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(resources, nameof(resources));

      var runner = new TestRunnerEx();
      var context = new MockDataProvider(resources);
      var data = runner.CreateContext(context);
      var processing = new TestProcessingContext();

      ITestReport report = null;
      try
      {
        report = runner.RunTest(test, data, processing);
      }
      catch (ResourceNotAvailableException)
      {
      }

      // run test
      Results = report.Results.All.ToList();
    }

    public IUnitTestResultsContext MustReturn(ITestResult testResult, ComparisonMode mode)
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