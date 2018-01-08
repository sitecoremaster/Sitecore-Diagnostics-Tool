namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  /// <summary>
  ///   Abstract class that implements ITest interface to simplify test development.
  /// </summary>
  public abstract class Test : ITest
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
    public virtual bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, IInstanceResourceContext data)
    {
      Assert.ArgumentNotNull(roles, nameof(roles));
      Assert.ArgumentNotNull(sitecoreVersion, nameof(sitecoreVersion));
      Assert.ArgumentNotNull(data, nameof(data));

      return IsActual(roles) && IsActual(sitecoreVersion) && IsActual(data.SitecoreInfo.ModulesInformation.InstalledModules) && IsActual(data);
    }

    protected virtual bool IsActual([NotNull] IReadOnlyCollection<ServerRole> roles)
    {
      return true;
    }

    protected virtual bool IsActual([NotNull] ISitecoreVersion sitecoreVersion)
    {
      return true;
    }

    protected virtual bool IsActual(IReadOnlyDictionary<string, IReleaseInfo> modules)
    {
      return true;
    }

    protected virtual bool IsActual([NotNull] IInstanceResourceContext data)
    {
      return true;
    }

    /// <summary>
    ///   All the test logic must be placed here. Use context parameter to access the API.
    /// </summary>
    /// <param name="data">An interface to test resources.</param>
    /// <param name="output">An interface to test output.</param>
    public abstract void Process(IInstanceResourceContext data, ITestOutputContext output);

    public bool IsActual(ISolutionResourceContext data, ISitecoreVersion sitecoreVersion)
    {
      return data.Values.Any(x => IsActual(x.ServerRoles, sitecoreVersion, x));
    }

    public void Process(ISolutionResourceContext data, ITestOutputContext output)
    {
      foreach (var instance in data.Values)
      {
        if (IsActual(instance.ServerRoles, instance.SitecoreInfo.SitecoreVersion, instance))
        {
          Process(instance, output);
        }
      }
    }
  }
}