namespace Sitecore.DiagnosticsTool.TestRunner
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public sealed class SolutionResourceContext : Dictionary<string, IInstanceResourceContext>, ISolutionResourceContext
  {
    public SolutionResourceContext([NotNull] IInstanceResourceContext[] dataContexts, [NotNull] ISystemContext system)
      : base(CreateDictionary(dataContexts))
    {
      System = system;
      Assert.ArgumentNotNull(dataContexts);
    }

    [NotNull]
    private static Dictionary<string, IInstanceResourceContext> CreateDictionary([NotNull] IEnumerable<IInstanceResourceContext> dataContexts)
    {
      Assert.ArgumentNotNull(dataContexts);

      var array = dataContexts.ToArray();

      try
      {
        return array.ToDictionary(x => x.SitecoreInfo.InstanceName, x => x);
      }
      catch (ArgumentException)
      {
        throw new SameNameInstancesAreNotSupportedException(array.First().SitecoreInfo.InstanceName);
      }
    }

    // TODO: rework 
    public ISitecoreDefaultsContext SitecoreDefaults => Values.First().SitecoreInfo.SitecoreDefaults;

    // TODO: rework 
    public ISitecoreVersion SitecoreVersion => Values.First().SitecoreInfo.SitecoreVersion;

    public ISystemContext System { get; }
  }
}