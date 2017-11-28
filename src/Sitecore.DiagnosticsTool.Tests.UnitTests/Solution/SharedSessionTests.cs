namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Solution
{
  using System.Collections.Specialized;
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

  public class SharedSessionTests : SharedSession
  {
    [Fact]
    public void Correct_SingleCluster_SingleInstance_InProc()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().FromXml(
              " <configuration>" +
              "   <sitecore>" +
              "     <tracking>" +
              "       <sharedSessionState defaultProvider=\"inproc123\">" +
              "         <providers>" +
              "           <add name=\"inproc123\" type=\"System.Web.SessionState.InProcSessionStateStore\" />" +
              "         </providers>" +
              "       </sharedSessionState>" +
              "     </tracking>" +
              "   </sitecore>" +
              " </configuration>")
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Correct_SingleCluster_TwoInstances()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test1",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create("/configuration/sitecore/tracking/sharedSessionState", new NameValueCollection()
              {
                { "defaultProvider", "mongo" }
              })
              .Add("/configuration/sitecore/tracking/sharedSessionState", "providers/add[@name='mongo' and @connectionString='cstr1']")
              .Add("/configuration", "/connectionStrings/add[@name='cstr1' and @connectionString='cstr1value']")
              .Add("/configuration/sitecore", "settings/setting[@name='Analytics.ClusterName' and @value='cluster1']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test2",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create("/configuration/sitecore/tracking/sharedSessionState", new NameValueCollection()
              {
                { "defaultProvider", "mongo" }
              })
              .Add("/configuration/sitecore/tracking/sharedSessionState", "providers/add[@name='mongo' and @connectionString='cstr1']")
              .Add("/configuration", "/connectionStrings/add[@name='cstr1' and @connectionString='cstr1value']")
              .Add("/configuration/sitecore", "settings/setting[@name='Analytics.ClusterName' and @value='cluster1']")
          })
        .Process(this)
        .Done();
    }

    [Fact]
    public void Bad_SingleCluster_TwoInstances()
    {
      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test1",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create("/configuration/sitecore/tracking/sharedSessionState", new NameValueCollection()
              {
                { "defaultProvider", "mongo" }
              })
              .Add("/configuration/sitecore/tracking/sharedSessionState", "providers/add[@name='mongo' and @connectionString='cstr1']")
              .Add("/configuration", "/connectionStrings/add[@name='cstr1' and @connectionString='cstr1value']")
              .Add("/configuration/sitecore", "settings/setting[@name='Analytics.ClusterName' and @value='cluster1']")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "test2",
            ServerRoles = new[] { ServerRole.ContentDelivery },
            Version = new SitecoreVersion(8, 2, 2),
            Configuration = new XmlDocument().Create("/configuration/sitecore/tracking/sharedSessionState", new NameValueCollection()
              {
                { "defaultProvider", "mongo" }
              })
              .Add("/configuration/sitecore/tracking/sharedSessionState", "providers/add[@name='mongo' and @connectionString='cstr1']")
              .Add("/configuration", "/connectionStrings/add[@name='cstr1' and @connectionString='cstr1value12345']")
              .Add("/configuration/sitecore", "settings/setting[@name='Analytics.ClusterName' and @value='cluster1']")
          })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetMessage(new Map<Map>()
        {
          {
            "cluster1", new Map()
            {
              { "test1", "cstr1value" },
              { "test2", "cstr1value12345" }
            }
          }
        })))
        .Done();
    }
  }
}