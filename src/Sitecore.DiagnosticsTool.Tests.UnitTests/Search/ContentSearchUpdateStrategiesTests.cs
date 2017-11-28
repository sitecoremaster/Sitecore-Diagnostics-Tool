namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Search
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.Search;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class ContentSearchUpdateStrategiesTests : ContentSearchUpdateStrategies
  {
    [Fact]
    public void Test1()
    {
      // no indexes, no errors
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Test2()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']/strategies/strategy") // index points to a strategy with empty name
            .Add("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']", "locations/crawler/Database", "core")
        })
        .Process(this)

        // .MustReturn(new TestOutput(TestResultState.Error, GetIndexStrategiesCorruptedMessage("index1"))) // DEBUG
        .Done();
    }

    [Fact]
    public void Test3()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']/strategies/strategy[@ref='contentSearch/indexUpdateStrategies/intervalAsyncCore']") // no strategy at all, so index refers to missing strategy
            .Add("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']", "locations/crawler/Database", "core")
        })
        .Process(this)

        // .MustReturn(new TestOutput(TestResultState.Error, GetIndexStrategiesCorruptedMessage("index1"))) // DEBUG
        .Done();
    }

    [Fact]
    public void Test4()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']/strategies/strategy[@ref='contentSearch/indexUpdateStrategies/intervalAsyncCore']")
            .Add("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']", "locations/crawler/Database", "core")
            .Add("/configuration/sitecore/contentSearch", "indexUpdateStrategies/intervalAsyncCore/param[@desc='database']") // broken strategy - no database
        })
        .Process(this)

        // .MustReturn(new TestOutput(TestResultState.Error, GetStrategyCorruptedMessage("intervalAsyncCore"))) // DEBUG
        .Done();
    }

    [Fact]
    public void Test5()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']/strategies/strategy[@ref='contentSearch/indexUpdateStrategies/intervalAsyncCore']")
            .Add("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']", "locations/crawler/Database", "core") // core index
            .Add("/configuration/sitecore/contentSearch", "indexUpdateStrategies/intervalAsyncCore/param[@desc='database']", "core") // core strategy
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Test6()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']/strategies/strategy[@ref='contentSearch/indexUpdateStrategies/intervalAsyncCore']")
            .Add("/configuration/sitecore/contentSearch/configuration/indexes/index[@id='index1']", "locations/crawler/Database", "web") // web index
            .Add("/configuration/sitecore/contentSearch", "indexUpdateStrategies/intervalAsyncCore/param[@desc='database']", "core") // core strategy
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetErrorMessage("index1", "web", "intervalAsyncCore", "core")))
        .Done();
    }
  }
}