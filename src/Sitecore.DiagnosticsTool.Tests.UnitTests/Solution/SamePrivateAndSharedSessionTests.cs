namespace Sitecore.DiagnosticsTool.Tests.UnitTests.Solution
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.Solution;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class SamePrivateAndSharedSessionTests : SharedPrivateSessionInterference
  {
    [Fact]
    public void Both_Shared_And_Private_Sessions_Use_Have_SessionType_Value()
    {
      /* worst case when both xdb and asp sessions have same sessionType value */

      new SolutionUnitTestContext()
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd1",
            ServerRoles = new[] { ServerRole.ContentDelivery }, // cd1 uses mongo shared session w/ sessionType1
            Version = new SitecoreVersion(8, 2, 2, 161221),
            Configuration = new XmlDocument().FromXml(
              " <configuration>" +
              "   <connectionStrings>" +
              "     <add name=\"session1\" connectionString=\"mongodb://someserver/session2\" />" + // session1 -> someserver/session2 (SAME! AAA)
              "   </connectionStrings>" +
              "   <sitecore>" +
              "     <settings>" +
              "       <setting name=\"Analytics.ClusterName\" value=\"cluster1\" />" + // cluster1
              "     </settings>" +
              "     <tracking>" +
              "       <sharedSessionState defaultProvider=\"mongo1\">" + // mongo1
              "         <providers>" +
              "           <add name=\"mongo1\" connectionStringName=\"session1\" sessionType=\"sessionType1\" />" + // type name is not important; session1; sessionType1 (SAME! BBB)
              "         </providers>" +
              "       </sharedSessionState>" +
              "     </tracking>" +
              "   </sitecore>" +
              "   <sessionState mode=\"\">" + // inproc1
              "     <providers>" +
              "       <add name=\"inproc1\" />" + // inproc1
              "     </providers>" +
              "   </sessionState>" +
              " </configuration>")
          })
        .AddInstance(
          new SitecoreInstance
          {
            InstanceName = "cd2",
            ServerRoles = new[] { ServerRole.ContentDelivery }, // cd2 uses mongo private session w/ sessionType1
            Version = new SitecoreVersion(8, 2, 2, 161221),
            Configuration = new XmlDocument().FromXml(
              " <configuration>" +
              "   <connectionStrings>" +
              "     <add name=\"session3\" connectionString=\"mongodb://someserver/session2\" />" + // session3 -> someserver/session2 (SAME! AAA)
              "   </connectionStrings>" +
              "   <sitecore>" +
              "     <settings>" +
              "       <setting name=\"Analytics.ClusterName\" value=\"cluster2\" />" + // cluster2
              "     </settings>" +
              "     <tracking>" +
              "       <sharedSessionState defaultProvider=\"inproc2\">" + // inproc2
              "         <providers>" +
              "           <add name=\"inproc2\" />" + // inproc2
              "         </providers>" +
              "       </sharedSessionState>" +
              "     </tracking>" +
              "   </sitecore>" +
              "   <sessionState mode=\"Custom\" customProvider=\"mongo2\" cookieless=\"false\" timeout=\"20\" sessionIDManagerType=\"Sitecore.SessionManagement.ConditionalSessionIdManager\">" +
              "     <providers>" +
              "       <add name=\"mongo2\" connectionStringName=\"session3\" sessionType=\"sessionType1\" />" + // type name is not important; session3; sessionType1 (SAME! BBB)
              "     </providers>" +
              "   </sessionState>" +
              " </configuration>")
          })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, GetMessage("#sessionType1", new[] { "cd2" }, new[] { "cd1" })))
        .Done();
    }
  }
}