namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;

  public sealed class SolutionTestResourceContext : Dictionary<string, ITestResourceContext>, ISolutionTestResourceContext
  {
    public SolutionTestResourceContext([NotNull] IEnumerable<ITestResourceContext> dataContexts)
      : base(CreateDictionary(dataContexts))
    {
      Assert.ArgumentNotNull(dataContexts);
    }

    [NotNull]
    private static Dictionary<string, ITestResourceContext> CreateDictionary([NotNull] IEnumerable<ITestResourceContext> dataContexts)
    {
      Assert.ArgumentNotNull(dataContexts);

      var array = dataContexts.ToArray();

      try
      {
        return array.ToDictionary(x => x.SitecoreInfo.InstanceName, x => x);
      }
      catch (ArgumentException)
      {
        throw new SameNameInstancesAreNotSupported(array.FirstOrDefault().SitecoreInfo.InstanceName);
      }
    }

    public string InstanceName { get; set; } = "";

    public ISystemContext System { get; } = new SystemContext();
  }
}