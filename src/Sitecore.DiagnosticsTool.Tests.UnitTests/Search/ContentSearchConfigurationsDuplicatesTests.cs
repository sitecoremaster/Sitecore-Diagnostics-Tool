namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Search
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.Search;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class ContentSearchConfigurationsDuplicatesTests : ContentSearchConfigurationsDuplicates
  {
    [Fact]
    public void CannotRun()
    {
      // no indexes, no errors
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.CannotRun, "Test is not actual for given conditions"))
        .Done();
    }

    [Fact]
    public void AllGood()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration[@type='Sitecore.ContentSearch.ContentSearchConfiguration, Sitecore.ContentSearch']"),
          IncludeFiles = new Map<ConfigurationFile>(x => x.FilePath)
          {
            { new ConfigurationFile("App_Config/Include/1.config", "<configuration><sitecore><contentSearch><configuration type='Sitecore.ContentSearch.ContentSearchConfiguration, Sitecore.ContentSearch' /></contentSearch></sitecore></configuration>") },
          }
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Problem()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration[@type='Sitecore.ContentSearch.ContentSearchConfiguration, Sitecore.ContentSearch']")
            .Add("/configuration/sitecore/contentSearch", "configuration[@type='Sitecore.ContentSearch.LuceneProvider.LuceneSearchConfiguration, Sitecore.ContentSearch.LuceneProvider']"),
          IncludeFiles = new Map<ConfigurationFile>(x => x.FilePath)
          {
            { new ConfigurationFile("App_Config/Include/1.config", "<configuration><sitecore><contentSearch><configuration type='Sitecore.ContentSearch.ContentSearchConfiguration, Sitecore.ContentSearch' /></contentSearch></sitecore></configuration>") },
            { new ConfigurationFile("App_Config/Include/2.config", "<configuration><sitecore><contentSearch><configuration type='Sitecore.ContentSearch.LuceneProvider.LuceneSearchConfiguration, Sitecore.ContentSearch.LuceneProvider' /></contentSearch></sitecore></configuration>") },
          }
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, "There are duplicates in ContentSearch configuration nodes that may lead to unpredictable behavior."))
        .Done();
    }
  }
}
