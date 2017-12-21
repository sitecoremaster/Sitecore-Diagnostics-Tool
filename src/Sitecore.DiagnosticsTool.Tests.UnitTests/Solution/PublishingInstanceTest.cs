namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Solution
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.Solution;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class PublishingInstanceTest : PublishingInstance
  {
    [Fact]
    public void Correct_NoPublishingInstance_SingleInstance()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='']")
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Correct_PublishingInstance_SingleInstance()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='test']")
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Correct_PublishingInstance_CmCd()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='cm']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='']")
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Correct_PublishingInstance_Cm1Cm2Cd()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm1",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='cm1']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm2",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='cm1']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='']")
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Wrong_PublishingInstance_Cm1Cm2Cd()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm1",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='cm1']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm2",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='cm2']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='']")
          })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetMessage(new Map<string[]>()
        {
          { "cm1", new[] { "cm1" } },
          { "cm2", new[] { "cm2" } }
        })))
        .Done();
    }

    [Fact]
    public void Wrong_PublishingInstance_Cm1Cm2()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm1",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='cm1']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cm2",
            ServerRoles = new[] { ServerRole.ContentManagement },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create($"/configuration/sitecore/settings/setting[@name='{PublishingInstanceSetting}' and @value='']")
          })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetMessage(new Map<string[]>()
        {
          { "cm1", new[] { "cm1" } },
          { "[empty]", new[] { "cm2" } }
        })))
        .Done();
    }
  }
}