namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Performance
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Performance;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class MediaLinkPrefixContainsTildeTests : MediaLinkPrefixContainsTilde
  {
    [Fact]
    public void Test()
    {
      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument().Create("/configuration"),
          Version = new SitecoreVersion(6, 6, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormat, KbNumber), Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Media.MediaLinkPrefix']"),
          Version = new SitecoreVersion(6, 6, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormat, KbNumber), Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Media.MediaLinkPrefix' and @value='~/media']"),
          Version = new SitecoreVersion(6, 6, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormat, KbNumber), Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Media.MediaLinkPrefix' and @value='/prefix/~/media']"),
          Version = new SitecoreVersion(6, 6, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormat, KbNumber), Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Media.MediaLinkPrefix' and @value='-/media']")
            .Add("/configuration/sitecore/settings", "setting[@name='Media.MediaLinkPrefix' and @value='/prefix/~/media']"),
          Version = new SitecoreVersion(6, 6, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, string.Format(ErrorFormat, KbNumber), Link))
        .Done();

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Configuration = new XmlDocument()
            .Create("/configuration/sitecore/settings/setting[@name='Media.MediaLinkPrefix' and @value='~/media']")
            .Add("/configuration/sitecore/settings", "setting[@name='Media.MediaLinkPrefix' and @value='-/media']"),
          Version = new SitecoreVersion(6, 6, 0)
        })
        .Process(this)
        .Done();
    }
  }
}