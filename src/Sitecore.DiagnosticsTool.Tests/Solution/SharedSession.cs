namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class SharedSession : SolutionTest
  {
    protected TypeRef InProcType { get; } = TypeRef.Parse("System.Web.SessionState.InProcSessionStateStore");

    public override string Name { get; } = "Shared Session is set up consistently";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    public override void Process(ISolutionTestResourceContext data, ITestOutputContext output)
    {
      var inprocMap = new Map<string[]>();
      var inconsistencyMap = new Map<Map>();

      var clusters = data.Values
        .Where(x => x.ServerRoles.Any(r => r == ServerRole.ContentDelivery))
        .Where(x => x.SitecoreInfo.IsAnalyticsEnabled)
        .GroupBy(i => i.SitecoreInfo.GetSetting("Analytics.ClusterName").EmptyToNull() ?? "[empty]")
        .ToDictionary(x => x.Key, x => x.ToArray());

      foreach (var cluster in clusters)
      {
        var clusterName = cluster.Key;
        var instances = cluster.Value;

        if (instances.Length == 0)
        {
          throw new NotImplementedException("impossible");
        }

        if (instances.Length == 1)
        {
          // single CD instance in cluster means we don't need to check anything
          output.Debug($"Cluster {clusterName.EmptyToNull() ?? "[empty]"} has single instance");

          continue;
        }

        var defaultProviders = instances
          .Select(x => new
          {
            x.SitecoreInfo,
            DefaultProviderName = x.SitecoreInfo.Configuration.SelectSingleElement("/configuration/sitecore/tracking/sharedSessionState").GetAttribute("defaultProvider")
          })
          .Select(x => new
          {
            x.SitecoreInfo,
            DefaultProvider = x.SitecoreInfo.Configuration.SelectSingleElement($"/configuration/sitecore/tracking/sharedSessionState/providers/add[@name='{x.DefaultProviderName}']")
          })
          .Select(x => new
          {
            x.SitecoreInfo,
            x.DefaultProvider,
            DefaultProviderType = TypeRef.Parse(x.DefaultProvider.GetAttribute("type"))
          })
          .ToArray();

        // check that all instances don't use InProc

        var gr = defaultProviders
          .GroupBy(x => x.DefaultProviderType)
          .ToArray();

        var instancesInProc = gr
          .FirstOrDefault(x => x.Key.Equals(InProcType))?
          .Select(x => x.SitecoreInfo.InstanceName)
          .ToArray();

        if (instancesInProc != null)
        {
          inprocMap.Add(clusterName, instancesInProc);

          continue;
        }

        // check that all instances use same shared session

        var data1 = defaultProviders
          .Select(x => new
          {
            x.SitecoreInfo,
            ConnectionStringName = x.DefaultProvider.GetAttribute("connectionString")
          })
          .ToMap(
            x => x.SitecoreInfo.InstanceName,
            x => x.SitecoreInfo.GetConnectionString(x.ConnectionStringName));

        var map = data1
          .GroupBy(x => x.Value)
          .ToArray();

        if (map.Length <= 1)
        {
          output.Debug($"Cluster {clusterName} has consistent shared session configuration, connection string: {map.FirstOrDefault()?.Key}");

          continue;
        }

        inconsistencyMap.Add(clusterName, data1);
      }

      if (inprocMap.Any())
      {
        var message = new ShortMessage(
          new Text($"InProc shared session mode is used among Sitecore instances which is not supported."),
          new BulletedList(inprocMap.Keys.ToArray(clusterName => new Container(
            new Text($"Cluster: {clusterName}"),
            BulletedList.Create(clusters[clusterName], instance => $"{instance.SitecoreInfo.InstanceName} - {IsAffected(inprocMap, clusterName)}")))));

        output.Error(message);
      }

      if (inconsistencyMap.Any())
      {
        var message = GetMessage(inconsistencyMap);

        output.Error(message);
      }
    }

    protected ShortMessage GetMessage(Map<Map> wrong)
    {
      // - cstr1: Cm1, Cm2
      // - cstr2: BadCm3
      // - [empty]: BadCm4
      var message = new ShortMessage(
        new Text("Out-of-proc shared session configuration is inconcistent: different Sitecore instances use different endpoints within same cluster:"),
        new BulletedList(wrong.ToArray(cluster => new Container(
          new Text(cluster.Key),
          BulletedList.Create(cluster.Value, x => $"{x.Key} - {x.Value}"))))
      );

      return message;
    }

    private string IsAffected(Map<string[]> inproc, string clusterName)
    {
      return inproc.ContainsKey(clusterName) ? "Affected (InProc)" : "Good";
    }
  }
}