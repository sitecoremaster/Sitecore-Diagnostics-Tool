namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Performance
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Performance;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class PublishEndHtmlClearingHandlersTests : PublishEndHtmlClearingHandlers
  {
    [Fact]
    public void PublishEndHtmlClearingHandlersTest_Passed()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.Publishing },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().TryParse(@"
<configuration>
  <sitecore>
    <events>
      <event name=""publish:end"">
        <handler type=""Sitecore.Publishing.HtmlCacheClearer, Sitecore.Kernel"" method=""ClearCache"">
          <sites hint=""list"">
            <site>website.site1</site>
            <site>website</site>
            <site>website.site2</site>
          </sites>
        </handler>
      </event>
    </events>
    <sites>
      <site name=""service"" />
      <site name=""modules_website"" />
      <site name=""website.site1"" />
      <site name=""website"" />
      <site name=""scheduler"" />
      <site name=""system"" />
      <site name=""website.site2"" />
    </sites>
  </sitecore>
</configuration>")
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void PublishEndHtmlClearingHandlersTest_Error()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          ServerRoles = new[] { ServerRole.Publishing },
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().TryParse(@"
<configuration>
  <sitecore>
    <events>
      <event name=""publish:end"">
        <handler type=""Sitecore.Publishing.HtmlCacheClearer, Sitecore.Kernel"" method=""ClearCache"">
          <sites hint=""list"">
            <site>website.site1</site>
          </sites>
        </handler>
      </event>
    </events>
    <sites>
      <site name=""shell"" />
      <site name=""login"" />
      <site name=""admin"" />
      <site name=""service"" />
      <site name=""modules_shell"" />
      <site name=""modules_website"" />
      <site name=""website.site1"" />
      <site name=""website.site2"" />
      <site name=""website"" />
      <site name=""scheduler"" />
      <site name=""system"" />
      <site name=""publisher"" />
    </sites>
  </sitecore>
</configuration>")
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("website.site2")))
        .MustReturn(new TestOutput(TestResultState.Warning, GetErrorMessage("website")))
        .Done();
    }
  }
}