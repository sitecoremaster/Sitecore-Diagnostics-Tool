namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;

  /// <summary>
  ///   Abstract class that implements ISolutionTest interface to simplify test development.
  /// </summary>
  public abstract class SolutionTest : ITest
  {
    /// <summary>
    ///   Easy to remember and share test name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    ///   The list of categories the test belongs to.
    /// </summary>
    public abstract IEnumerable<Category> Categories { get; }

    /// <summary>
    ///   The method indicates if this specific test is actual for Sitecore version of the instance under test.
    /// </summary>
    public virtual bool IsActual(ISolutionResourceContext data, ISitecoreVersion sitecoreVersion)
    {
      Assert.ArgumentNotNull(data, nameof(data));
      Assert.ArgumentNotNull(sitecoreVersion, nameof(sitecoreVersion));

      return IsActual(sitecoreVersion) && IsActual(data);
    }

    protected virtual bool IsActual([NotNull] ISolutionResourceContext data)
    {
      return true;
    }

    protected virtual bool IsActual([NotNull] ISitecoreVersion sitecoreVersion)
    {
      return true;
    }

    /// <summary>
    ///   All the test logic must be placed here. Use data parameter to access the resources API, use output parameter to give
    ///   results.
    /// </summary>
    /// <param name="data">An interface to test resources.</param>
    /// <param name="output">An interface to test output.</param>
    public abstract void Process(ISolutionResourceContext data, ITestOutputContext output);
  }
}