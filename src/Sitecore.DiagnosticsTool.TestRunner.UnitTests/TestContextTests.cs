// ReSharper disable CodeAnnotationAnalyzer

namespace Sitecore.DiagnosticsTool.TestRunner.UnitTests
{
  using System;
  using System.Reflection;
  using FluentAssertions;
  using Microsoft.VisualStudio.TestTools.UnitTesting;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.UnitTests.Common.DataProviders;

  [TestClass]
  public class TestContextTests
  {
    [TestMethod]
    public void TestContextNotNull()
    {
      var testRunner = new TestRunner();
      var test = new ContextNotNullTest();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var context = new TestProcessingContext();
      test.Process(data, context);
    }

    [TestMethod]
    public void TestContextResourcesNotNull()
    {
      var test = new ResourcesNotNullTest();
      var testRunner = new TestRunner();
      var data = testRunner.CreateContext(new EmptyDataProvider(new SitecoreVersion(8, 2, 2, 161221)));
      var context = new TestProcessingContext();
      test.Process(data, context);
    }

    public class ContextNotNullTest : FakeTest
    {
      /// <summary>
      ///   All the test logic must be placed here. Use context parameter to access the API.
      /// </summary>
      /// <param name="data">A single interface to access API.</param>
      // ReSharper disable once CodeAnnotationAnalyzer
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        data.Should().NotBeNull();
      }
    }

    public class ResourcesNotNullTest : FakeTest
    {
      /// <summary>
      ///   All the test logic must be placed here. Use context parameter to access the API.
      /// </summary>
      /// <param name="data">A single interface to access API.</param>
      public override void Process(ITestResourceContext data, ITestOutputContext output)
      {
        var resourceContext = (ITestResourceContext)data;
        var exception = typeof(ResourceNotAvailableException);

        new Action(() => resourceContext.FileSystem.IsNotNull("resourceContext.FileSystem"))
          .ShouldThrow<ResourceNotAvailableException>()
          .WithMessage(ResourceType.FileSystem + " resource is not available");
        new Action(() => resourceContext.SitecoreInfo.DataFolderPath.IsNotNull("resourceContext.SitecoreInfo"))
          .ShouldThrow<ResourceNotAvailableException>()
          .WithMessage(ResourceType.SitecoreInformation + " (Data Folder Path) resource is not available");
        new Action(() => resourceContext.WebServer.IsNotNull("resourceContext.WebServer"))
          .ShouldThrow<ResourceNotAvailableException>()
          .WithMessage(ResourceType.WebServer + " resource is not available");
        new Action(() => resourceContext.Logs.IsNotNull("resourceContext.Logs"))
          .ShouldThrow<ResourceNotAvailableException>()
          .WithMessage(ResourceType.LogFiles + " resource is not available");
        new Action(() => resourceContext.Databases.IsNotNull("resourceContext.SqlDatabases"))
          .ShouldThrow<ResourceNotAvailableException>()
          .WithMessage(ResourceType.Database + " resource is not available");

        // just for case somebody adding new resource will forget to add it here
        // make an additional loop using reflection
        foreach (var resourceProperty in TestContextHelper.GetContextIResourceProperties())
        {
          try
          {
            resourceProperty.GetValue(data, null).IsNotNull(resourceProperty.Name);
          }
          catch (TargetInvocationException ex)
          {
            if (ex.InnerException.GetType() == exception)
            {
              continue;
            }

            throw new InvalidOperationException(string.Format("The call should have returned {0} exception but the {1} was thrown instead. " + ex.Message + Environment.NewLine + ex.StackTrace, exception.FullName, ex.GetType().FullName));
          }

          // ISitecoreInformationContext is required for test to run
          if (resourceProperty.PropertyType != typeof(ISitecoreInformationContext))
          {
            throw new InvalidOperationException($"The {exception.FullName} exception wasn't thrown. ");
          }
        }
      }
    }
  }
}