namespace Sitecore.DiagnosticsTool.TestRunner.UnitTests
{
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public abstract class FakeTest : ITest
  {
    public bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, ITestResourceContext data) => true;

    public abstract void Process(ITestResourceContext data, ITestOutputContext output);

    public string Name => "";

    public IEnumerable<Category> Categories => Enumerable.Empty<Category>();
  }
}