namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public abstract class TestRunnerBase<TTest, TResource, TDataSource>
    where TTest : ITestMetadata where TResource : IInstanceName
  {
    protected virtual bool UnitTesting { get; } = false;

    [NotNull]
    public IEnumerable<ITestReport> RunTests([NotNull] IEnumerable<TTest> tests, [NotNull] TDataSource dataSource, [CanBeNull] Action<ITestMetadata> onTestRun = null)
    {
      Assert.ArgumentNotNull(tests, nameof(tests));
      Assert.ArgumentNotNull(dataSource, nameof(dataSource));

      var process = new TestProcessingContext();
      var data = CreateResoureContext(dataSource);
      foreach (var testReport in RunTests(tests, data, process, onTestRun))
      {
        yield return testReport;
      }
    }

    [NotNull]
    public IEnumerable<ITestReport> RunTests([NotNull] IEnumerable<TTest> tests, [NotNull] TResource data, [NotNull] TestProcessingContext process, [CanBeNull] Action<ITestMetadata> onTestRun = null)
    {
      Assert.ArgumentNotNull(tests, nameof(tests));
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(process, nameof(process));

      foreach (var test in tests)
      {
        if (test == null)
        {
          continue;
        }

        onTestRun?.Invoke(test);

        // run test
        yield return RunTest(test, data, process);

        // reset context instead of creating new one
        process.Reset();
      }
    }

    public ITestReport RunTest(TTest test, TResource data, ITestProcessingContext context)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(context, nameof(context));

      var stopwatch = new Stopwatch();
      stopwatch.Start();

      try
      {
        var report = DoRunTest(test, data, context);
        stopwatch.Stop();
        report.ExecutionTime = stopwatch.Elapsed;

        return report;
      }
      finally
      {
        stopwatch.Stop();
      }
    }

    [NotNull]
    private ITestReport DoRunTest([NotNull] TTest test, [NotNull] TResource data, [NotNull] ITestProcessingContext context)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(context, nameof(context));

      try
      {
        if (!IsTestActual(test, data))
        {
          if (!context.Results.All.Any())
          {
            context.Results.Add(new TestOutput(TestResultState.CannotRun, "Test is not actual for given conditions"));
          }

          return CreateReport(test, context, data.InstanceName);
        }
      }
      catch (ResourceNotAvailableException ex)
      {
        if (!context.Results.All.Any())
        {
          context.Results.Add(new TestOutput(TestResultState.CannotRun, $"Test failed to run due to missing resource: {ex.Message}", null, ex.PrintException()));
        }

        return CreateReport(test, context, data.InstanceName);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Test failed with unhandled exception");

        context.Results.Add(new TestOutput(TestResultState.CannotRun, "Checking test preconditions failed with unhandled exception. " + ex.Message.TrimEnd('.') + ". Find details in the log file.", null, ex.PrintException()));
      }

      return RunTestInner(test, data, context);
    }

    [NotNull]
    private ITestReport RunTestInner([NotNull] TTest test, [NotNull] TResource data, [NotNull] ITestProcessingContext context)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(context, nameof(context));

      try
      {
        ProcessTest(test, data, context);
      }
      catch (ResourceNotAvailableException ex)
      {
        if (!context.Results.All.Any())
        {
          context.Results.Add(new TestOutput(TestResultState.CannotRun, $"Test failed to run due to missing resource: {ex.Message}", null, ex.PrintException()));
        }
      }
      catch (Exception ex)
      {
        if (UnitTesting)
        {
          throw;
        }

        Log.Error(ex, "Test failed with unhandled exception");

        context.Results.Add(new TestOutput(TestResultState.CannotRun, "Test failed with unhandled exception. " + ex.Message.TrimEnd('.') + ". Find details in the log file.", null, new DetailedMessage(new CodeBlock(ex.PrintException()))));
      }

      return CreateReport(test, context, data.InstanceName);
    }

    public ITestResourceContext CreateContext(IDataProvider dataProivder)
    {
      Assert.ArgumentNotNull(dataProivder, nameof(dataProivder));

      var context = new TestResourceContext(dataProivder.InstanceName);
      var properties = TestContextHelper.GetContextResourceProperties().ToArray();
      Assert.IsTrue(properties.Length > 0, "No resources found on ITestResourceContext");

      foreach (var resource in dataProivder.GetResources())
      {
        var resourceAssigned = false;
        var resourceType = resource.GetType();
        foreach (var propertyInfo in properties)
        {
          if (propertyInfo.PropertyType.IsAssignableFrom(resourceType))
          {
            propertyInfo.SetValue(context, resource, null);

            resourceAssigned = true;
            break;
          }
        }

        if (!resourceAssigned)
        {
          Log.Error("The resource is not assigned: " + resourceType.FullName);
        }
      }

      // we need to call data.SitecoreInfo only, 
      // Major check is fake to prevent optimization remove "unnecessary" call
      if (context.SitecoreInfo.SitecoreVersion.Major < -2475)
      {
        throw new NotSupportedException();
      }

      return context;
    }

    [NotNull]
    private ITestReport CreateReport([NotNull] TTest test, [NotNull] ITestProcessingContext context, [CanBeNull] string instanceName)
    {
      Assert.ArgumentNotNull(test, nameof(test));
      Assert.ArgumentNotNull(context, nameof(context));

      return new TestReport(test, context.Results, instanceName);
    }

    protected abstract bool IsTestActual(TTest test, TResource data);

    protected abstract void ProcessTest(TTest test, TResource data, ITestProcessingContext context);

    protected abstract TResource CreateResoureContext(TDataSource data);
  }
}