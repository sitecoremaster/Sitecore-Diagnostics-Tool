namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Performance
{
  using System.Xml;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Performance;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class CompilationDebugDisabledTests : CompilationDebugDisabled
  {
    [Fact]
    public void Test()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().TryParse("<configuration />")
        })
        .Process(this)
        .Done();

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(XPath)
        })
        .Process(this)
        .Done();

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(XPath + "[@debug='false']")
        })
        .Process(this)
        .Done();

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(8, 2, 2),
          Configuration = new XmlDocument().Create(XPath + "[@debug='true']")
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, MessageCompilationDebugEnabled))
        .Done();
    }
  }
}