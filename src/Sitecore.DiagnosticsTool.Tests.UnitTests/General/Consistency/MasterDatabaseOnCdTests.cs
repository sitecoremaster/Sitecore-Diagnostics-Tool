namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Consistency
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Consistency;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class MasterDatabaseOnCdTests : MasterDatabaseOnCd
  {
    [Fact]
    public void Passed()
    {
      new SolutionUnitTestContext()
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create("/configuration")
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Warn_Sites()
    {
      new SolutionUnitTestContext()
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(SiteXPath)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<sites>")))
        .Done();
    }

    [Fact]
    public void CannotRun_Site()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentManagement },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(SiteXPath)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.CannotRun, "Test is not actual for given conditions"))
        .Done();
    }

    [Fact]
    public void Warn_IDTable()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(IdTableXPath)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<IDTable>")))
        .Done();
    }

    [Fact]
    public void Warn_Databases()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(DatabaseXPath)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<databases>")))
        .Done();
    }

    [Fact]
    public void Warn_Search()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(SearchXPath)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<search>")))
        .Done();
    }

    [Fact]
    public void Warn_Scheduling()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(SchedulingXPath1)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<scheduling>")))
        .Done();
    }

    [Fact]
    public void Warn_Scheduling2()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.ContentDelivery },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(SchedulingXPath2, "master")
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("<scheduling>")))
        .Done();
    }
  }
}