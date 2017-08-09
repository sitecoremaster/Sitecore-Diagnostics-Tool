namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Performance
{
  using System.Xml;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Performance;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;
  using Xunit;

  public class DefaultCacheSizesTest : DefaultCacheSizes
  {
    private const string WebDatabaseXPath = "/configuration/sitecore/databases/database[@id='web']";
    private const string CacheSizesXPath = WebDatabaseXPath + "/cacheSizes";

    [Fact]
    public void TestPassed()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create(CacheSizesXPath)
            .Add(CacheSizesXPath, "data", "21MB")
            .Add(CacheSizesXPath, "items", "11MB")
            .Add(CacheSizesXPath, "paths", "500KB")
            .Add(CacheSizesXPath, "itempaths", "10MB")
            .Add(CacheSizesXPath, "standardValues", "500KB")
            .Add(WebDatabaseXPath, PrefetchXPath, "11MB"),
          Version = new SitecoreVersion(7, 1, 1, 140130)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void TestFailData()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create(CacheSizesXPath)
            .Add(CacheSizesXPath, "data", "20MB")
            .Add(CacheSizesXPath, "items", "10MB")
            .Add(CacheSizesXPath, "paths", "500KB")
            .Add(CacheSizesXPath, "itempaths", "10MB")
            .Add(CacheSizesXPath, "standardValues", "500KB")
            .Add(WebDatabaseXPath, PrefetchXPath, "10MB"),
          Version = new SitecoreVersion(7, 1, 1, 140130)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetMessage(new Map<Map>()
        {
          {
            "web", new Map
            {
              { "data", "20MB" },
              { "items", "10MB" },
              { "prefetch", "10MB" }
            }
          }
        })))
        .Done();
    }
  }
}