namespace Sitecore.DiagnosticsTool.Tests.General
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class SystemTest : SolutionTest
  {
    public override string Name { get; } = "System information";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ISolutionTestResourceContext data, ITestOutputContext output)
    {
      // ReSharper disable once AssignNullToNotNullAttribute
      output.Debug("ApplicationInfo: " + data.System.ApplicationInfo);
    }
  }
}