namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class DiagCodeTests : DiagCode
  {
    private static readonly SitecoreVersion ProductVersion = new SitecoreVersion(7, 2, 6, 160123);

    [Fact]
    public void DefaultPassed1Test()
    {
      var configuration = new SitecoreDefaultsContext(ProductVersion).Configuration;

      var setting = "AliasesActive";
      var expectedValue = "true";

      // actualValue = true from Sitecore defaults

      var code = $"<tests><setting name=\"{setting}\" mode=\"default\" /></tests>";

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Configuration = configuration
        })
        .AddResource(new SystemContext(code))
        .Process(this)
        .Done();
    }

    [Fact]
    public void DefaultPassed2Test()
    {
      var configuration = new SitecoreDefaultsContext(ProductVersion).Configuration;

      var setting = "AliasesActive";
      var expectedValue = "true";
      var actualValue = "true";

      AddSetting(configuration, setting, actualValue);

      var code = $"<tests><setting name=\"{setting}\" mode=\"default\" /></tests>";

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Configuration = configuration
        })
        .AddResource(new SystemContext(code))
        .Process(this)
        .Done();
    }

    [Fact]
    public void DefaultErrorTest()
    {
      var configuration = new SitecoreDefaultsContext(ProductVersion).Configuration;

      var setting = "AliasesActive";
      var actualValue = "false";
      var expectedValue = "true";

      AddSetting(configuration, setting, actualValue);

      var code = $"<tests><setting name=\"{setting}\" mode=\"default\" /></tests>";

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Configuration = configuration
        })
        .AddResource(new SystemContext(code))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, $"Setting \"{setting}\" value is \"{actualValue}\" which differs from default \"{expectedValue}\""))
        .Done();
    }

    [Fact]
    public void EqualsPassed1Test()
    {
      var configuration = new SitecoreDefaultsContext(ProductVersion).Configuration;

      var setting = "AliasesActive";
      var expectedValue = "true";

      // actualValue = true from Sitecore defaults

      var code = $"<tests><setting name=\"{setting}\" value=\"{expectedValue}\" /></tests>";

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Configuration = configuration
        })
        .AddResource(new SystemContext(code))
        .Process(this)
        .Done();
    }

    [Fact]
    public void EqualsPassed2Test()
    {
      var configuration = new SitecoreDefaultsContext(ProductVersion).Configuration;

      var setting = "AliasesActive";
      var expectedValue = "true";
      var actualValue = "true";

      AddSetting(configuration, setting, actualValue);

      var code = $"<tests><setting name=\"{setting}\" value=\"{expectedValue}\" /></tests>";

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Configuration = configuration
        })
        .AddResource(new SystemContext(code))
        .Process(this)
        .Done();
    }

    [Fact]
    public void EqualsErrorTest()
    {
      var configuration = new SitecoreDefaultsContext(ProductVersion).Configuration;

      var setting = "AliasesActive";
      var actualValue = "false";
      var expectedValue = "true";

      AddSetting(configuration, setting, actualValue);

      var code = $"<tests><setting name=\"{setting}\" value=\"{expectedValue}\" /></tests>";

      UnitTestContext
        .Create(this)
        .AddResource(new SitecoreInstance
        {
          Version = ProductVersion,
          Configuration = configuration
        })
        .AddResource(new SystemContext(code))
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, $"Setting \"{setting}\" value is \"{actualValue}\" which differs from expected \"{expectedValue}\""))
        .Done();
    }

    private static void AddSetting(XmlDocument configuration, string settingName, string settingValue)
    {
      var settings = (XmlElement)configuration.SelectSingleNode("/configuration/sitecore/settings");
      var setting = configuration.CreateElement("setting");
      setting.SetAttribute("name", settingName);
      setting.SetAttribute("value", settingValue);
      settings.AppendChild(setting);
    }
  }
}