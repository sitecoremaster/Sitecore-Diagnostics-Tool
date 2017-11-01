namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Performance
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
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

    private SitecoreVersion Sc711 { get; } = new SitecoreVersion(7, 1, 1, 140130);

    [Fact]
    public void TestPassed()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create(CacheSizesXPath)
            .Add(CacheSizesXPath, "data", "210MB")
            .Add(CacheSizesXPath, "items", "11MB")
            .Add(CacheSizesXPath, "paths", "501KB")
            .Add(CacheSizesXPath, "itempaths", "11MB")
            .Add(CacheSizesXPath, "standardValues", "501KB")
            .Add(WebDatabaseXPath, PrefetchXPath, "11MB"),
          Version = Sc711
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void TestFailData()
    {
      var data = new Map
      {
        {"data", "20MB"},
        {"items", "10MB"},

        // {"paths", "500KB"}, // no itempaths to make it fallback to default cache size 100KB 
        {"itempaths", "1MB"},
        {"standardValues", "50KB"},
        {"prefetch", "1MB"}
      };

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create(CacheSizesXPath)
            .Add(CacheSizesXPath, "data", data["data"])
            .Add(CacheSizesXPath, "items", data["items"])

            //.Add(CacheSizesXPath, "paths", data["paths"])
            .Add(CacheSizesXPath, "itempaths", data["itempaths"])
            .Add(CacheSizesXPath, "standardValues", data["standardValues"])
            .Add(WebDatabaseXPath, PrefetchXPath, data["prefetch"]),
          Version = Sc711
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetMessage(new Map<Map<CacheSizeDetails>>
        {
          {
            "web", new Map<CacheSizeDetails>
            {
              {"data", new CacheSizeDetails {Value = CacheSize.Parse(data["data"], Sc711), Comment = "which is default"}},
              {"items", new CacheSizeDetails {Value = CacheSize.Parse(data["items"], Sc711), Comment = "which is default"}},
            }
          }
        }, $"One or several Sitecore caches are not tuned up and use default settings which may lead to performance degradation:")))
        .MustReturn(new TestOutput(TestResultState.Error, GetMessage(new Map<Map<CacheSizeDetails>>
        {
          {
            "web", new Map<CacheSizeDetails>
            {
              {"paths", new CacheSizeDetails {Value = CacheSize.Parse("100KB", Sc711), Comment = "cache size is not specified, uses Caching.DefaultPathCacheSize setting value as fallback"}},
              {"standardValues", new CacheSizeDetails {Value = CacheSize.Parse(data["standardValues"], Sc711), Comment = $"which is below than default: {Size.FromKB(500)}"}},
              {"itempaths", new CacheSizeDetails {Value = CacheSize.Parse(data["itempaths"], Sc711), Comment = $"which is below than default: {Size.FromMB(10)}"}},
              {"prefetch", new CacheSizeDetails {Value = CacheSize.Parse(data["prefetch"], Sc711), Comment = $"which is below than default: {Size.FromMB(10)}"}}
            }
          }
        }, $"One or several Sitecore caches are use custom configuration which is below the minimum recommended values (set up by default) which may lead to performance degradation:")))
        .Done();
    }
  }
}