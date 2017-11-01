namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using System;
  using System.Collections.Generic;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class CompilationDebugDisabled : Test
  {
    [NotNull]
    protected const string MessageCompilationDebugEnabled = "The ASP.NET debugging is enabled (<compilation debug=\"true\" ... />), it should be disabled in production environment";

    protected const string XPath = @"/configuration/compilation";

    public override string Name { get; } = "The ASP.NET debugging must be disabled on production";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Production};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      // Get resources first
      var configuration = data.SitecoreInfo.Configuration;

      // Perform checks
      var compilation = configuration.SelectSingleNode(XPath) as XmlElement;
      if (compilation == null)
      {
        return;
      }

      var debug = compilation.GetAttribute("debug");
      if (debug.Equals("true", StringComparison.OrdinalIgnoreCase))
      {
        output.Warning(MessageCompilationDebugEnabled);
      }
    }
  }
}