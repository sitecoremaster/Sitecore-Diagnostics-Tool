namespace Sitecore.DiagnosticsTool.TestRunner.UnitTests
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.UnitTests.Common.DataProviders;

  [TestClass]
  public class TestResultsTests
  {
    [TestMethod]
    public void TestReportResults()
    {
      var testRunner = new TestRunner();
      var test = new PassedTestITest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(7, 2, 2, 140526)));
      var report = testRunner.RunTest(test, data, processing);

      report.Should().NotBeNull();
      report.Owner.Should().NotBeNull();
      report.Results.Should().NotBeNull();
      report.ExecutionTime.Ticks.Should().BeGreaterThan(0);
    }

    [TestMethod]
    public void TestReportResultsCannotRun()
    {
      var testRunner = new TestRunner();
      var test = new CannotStartTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var report = testRunner.RunTest(test, data, processing);

      report.Results.Errors.Count().Should().Be(0);
      report.Results.Warnings.Count().Should().Be(0);
      report.Results.CannotRun.Count().Should().Be(1);
      report.Results.DebugLogs.Count().Should().Be(0);

      report.Results.CannotRun.Single().Message.Should().Be(ResourceType.FileSystem + " resource is not available");
    }

    [TestMethod]
    public void TestReportResultsError()
    {
      var testRunner = new TestRunner();
      var test = new ErrorTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var report = testRunner.RunTest(test, data, processing);

      report.Results.Errors.Count().Should().Be(1);
      report.Results.Warnings.Count().Should().Be(0);
      report.Results.CannotRun.Count().Should().Be(0);
      report.Results.DebugLogs.Count().Should().Be(0);

      report.Results.Errors.Single().Message.Should().Be("Error-Expected");
    }

    [TestMethod]
    public void TestReportResultsNotActual()
    {
      var testRunner = new TestRunner();
      var test = new NonActualTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(7, 2, 2, 140526)));
      var report = testRunner.RunTest(test, data, processing);

      report.Results.Errors.Count().Should().Be(0);
      report.Results.Warnings.Count().Should().Be(0);
      report.Results.CannotRun.Count().Should().Be(0);
      report.Results.DebugLogs.Count().Should().Be(0);
    }

    [TestMethod]
    public void TestReportResultsUnidentified()
    {
      var testRunner = new TestRunner();
      var test = new UnidentifiedTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var report = testRunner.RunTest(test, data, processing);

      report.Results.Errors.Count().Should().Be(0);
      report.Results.Warnings.Count().Should().Be(0);
      report.Results.CannotRun.Count().Should().Be(2);
      report.Results.DebugLogs.Count().Should().Be(2);
    }

    [TestMethod]
    public void TestReportResultsPassed()
    {
      var testRunner = new TestRunner();
      var test = new PassedTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var report = testRunner.RunTest(test, data, processing);

      report.Results.Errors.Count().Should().Be(0);
      report.Results.Warnings.Count().Should().Be(0);
      report.Results.CannotRun.Count().Should().Be(0);
      report.Results.DebugLogs.Count().Should().Be(0);
    }

    [TestMethod]
    public void TestReportResultsWarnings()
    {
      var testRunner = new TestRunner();
      var test = new WarningTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var report = testRunner.RunTest(test, data, processing);

      report.Results.Errors.Count().Should().Be(0);
      report.Results.Warnings.Count().Should().Be(1);
      report.Results.CannotRun.Count().Should().Be(0);
      report.Results.DebugLogs.Count().Should().Be(0);

      report.Results.Warnings.Single().Message.Should().Be("Warning-Expected");
    }

    [TestMethod]
    public void TestReport_OneSecondTest()
    {
      var testRunner = new TestRunner();
      var test = new OneSecondRunTest();
      var processing = new TestProcessingContext();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var report = testRunner.RunTest(test, data, processing);

      report.ExecutionTime.TotalMilliseconds.Should().BeGreaterOrEqualTo(new TimeSpan(0, 0, 0, 1).TotalMilliseconds);
    }

    public class ErrorTest : FakeTest
    {
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));
        output.Error("Error-Expected");
      }
    }

    public class NonActualTest : ITest
    {
      public void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));
        output.Error("Error-Expected");
      }

      public IEnumerable<ServerRole> ServerRoles { get; }

      public bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, ITestResourceContext data)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(sitecoreVersion, nameof(sitecoreVersion));

        return false;
      }

      public string Name { get; private set; }

      public IEnumerable<Category> Categories { get; private set; }
    }

    public class WarningTest : FakeTest
    {
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));
        output.Warning("Warning-Expected");
      }
    }

    public class PassedTest : FakeTest
    {
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));
      }
    }

    public class PassedTestITest : ITest
    {
      public void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));
      }

      public IEnumerable<ServerRole> ServerRoles { get; }

      public bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, ITestResourceContext data)
      {
        return true;
      }

      /// <summary>
      ///   Easy to remember and share test name.
      /// </summary>
      public string Name => "Some Name";

      /// <summary>
      ///   The list of categories the test belongs to.
      /// </summary>
      public IEnumerable<Category> Categories => new[] { Category.General };
    }

    public class UnidentifiedTest : FakeTest
    {
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        throw new InvalidOperationException("Exception-Expected.");
      }
    }

    public class CannotStartTest : FakeTest
    {
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));

        var fileSystem = data.FileSystem;
        throw new InvalidOperationException("This code must not be reached" + fileSystem);
      }
    }

    public class OneSecondRunTest : FakeTest
    {
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        Diagnostics.Base.Assert.ArgumentNotNull(data, nameof(data));

        Thread.Sleep(1050); // 1050 is because the value is not very accurate
      }
    }
  }
}