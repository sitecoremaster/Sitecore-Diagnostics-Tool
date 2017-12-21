namespace Sitecore.DiagnosticsTool.Tests.UnitTests.General.Consistency
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;
  using Sitecore.DiagnosticsTool.Tests.General.Consistency;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Context;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources;

  using Xunit;

  public class GlobalAsaxInheritsFromTests : GlobalAsaxInheritsFrom
  {
    [Fact]
    public void RunTestNotActual()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(6, 4, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.CannotRun, "Test is not actual for given conditions"))
        .Done();
    }

    [Fact]
    public void RunTest1()
    {
      const string Header = "<%@  ApplicatiON LANGUAGE = 'C#' Inherits = \"Sitecore.Web.Application \" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTest2()
    {
      const string Header = "<%@Application Language='C#' Inherits=\"Sitecore.ContentSearch.SolrProvider.CastleWindsorIntegration.WindsorApplication\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTest3()
    {
      const string Header = "<%@Application Language='C#' Inherits=\"Sitecore.ContentSearch.SolrProvider.AutoFacIntegration.AutoFacApplication\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTest4()
    {
      const string Header = "<%@Application Language='C#' Inherits=\"Sitecore.ContentSearch.SolrProvider.NinjectIntegration.NinjectApplication\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTest5()
    {
      const string Header = "<%@Application Language='C#' Inherits=\"Sitecore.ContentSearch.SolrProvider.StructureMapIntegration.StructureMapApplication\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTest6()
    {
      const string Header = "<%@Application Language='C#' Inherits=\"Sitecore.ContentSearch.SolrProvider.UnityIntegration.UnityApplication\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTest7()
    {
      const string Header = "<%@Application Language='C#' Inherits=\"System.Web.HttpApplication\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, SystemWebMessage))
        .Done();
    }

    [Fact]
    public void RunTest8()
    {
      const string Header = "<%@Application Language='C#' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, SystemWebMessage))
        .Done();
    }

    [Fact]
    public void RunTest9()
    {
      var header = "<%@Application Language='C#' Inherits=\"Sitecore.Web.Application2\" %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(header)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, SystemWebMessage), ComparisonMode.StartsWith)
        .Done();
    }

    [NotNull]
    protected string GetGlobalAsaxFile(string header)
    {
      const string GlobalFileBody = @"
      <script runat=\""server\"">
        public void Application_Start() {
        }

        public void Application_End() {
        }

        public void Application_Error(object sender, EventArgs args) {
        }

      </script>
      ";

      return header + GlobalFileBody;
    }

    [Fact]
    public void RunTestSingleQuoteNotActual()
    {
      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(6, 4, 0)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.CannotRun, "Test is not actual for given conditions"))
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote1()
    {
      const string Header = "<%@  ApplicatiON LANGUAGE = 'C#' Inherits = 'Sitecore.Web.Application ' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote2()
    {
      const string Header = "<%@Application Language='C#' Inherits='Sitecore.ContentSearch.SolrProvider.CastleWindsorIntegration.WindsorApplication' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote3()
    {
      const string Header = "<%@Application Language='C#' Inherits='Sitecore.ContentSearch.SolrProvider.AutoFacIntegration.AutoFacApplication' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote4()
    {
      const string Header = "<%@Application Language='C#' Inherits='Sitecore.ContentSearch.SolrProvider.NinjectIntegration.NinjectApplication' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote5()
    {
      const string Header = "<%@Application Language='C#' Inherits='Sitecore.ContentSearch.SolrProvider.StructureMapIntegration.StructureMapApplication' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote6()
    {
      const string Header = "<%@Application Language='C#' Inherits='Sitecore.ContentSearch.SolrProvider.UnityIntegration.UnityApplication' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote7()
    {
      const string Header = "<%@Application Language='C#' Inherits='System.Web.HttpApplication' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, SystemWebMessage))
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote8()
    {
      const string Header = "<%@Application Language='C#' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(Header)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Error, SystemWebMessage))
        .Done();
    }

    [Fact]
    public void RunTestSingleQuote9()
    {
      var header = "<%@Application Language='C#' Inherits='Sitecore.Web.Application2' %>";

      new SolutionUnitTestContext()
        
        .AddInstance(new SitecoreInstance
        {
          Version = new SitecoreVersion(7, 2, 2),
          GlobalAsaxFile = GetGlobalAsaxFile(header)
        })
        .Process(this)
        .MustReturn(new TestOutput(TestResultState.Warning, SystemWebMessage + Comment))
        .Done();
    }
  }
}