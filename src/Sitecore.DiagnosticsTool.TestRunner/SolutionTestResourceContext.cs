namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public sealed class SolutionTestResourceContext : Dictionary<string, ITestResourceContext>, ISolutionTestResourceContext
  {
    public SolutionTestResourceContext([NotNull] ITestResourceContext[] dataContexts, [NotNull] ISystemContext system)
      : base(CreateDictionary(dataContexts))
    {
      System = system;
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
        throw new SameNameInstancesAreNotSupported(array.First().SitecoreInfo.InstanceName);
      }
    }

    public ISystemContext System { get; }
  }
}