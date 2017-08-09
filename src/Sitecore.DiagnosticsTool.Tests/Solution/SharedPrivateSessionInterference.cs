namespace Sitecore.DiagnosticsTool.Tests.Solution
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class SharedPrivateSessionInterference : SolutionTest
  {
    public override string Name { get; } = "Shared and Private sessions interference";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Analytics };

    public override void Process(ISolutionTestResourceContext data, ITestOutputContext output)
    {
      var errors = new List<string>();

      // get a dictionary where key is instance's SitecoreInfo, and values are shared session state providers' XML configuration 
      var sharedProviders = data.Values // identifier is a combination of connection string and sessionType value e.g. shared or private or any custom string
        .Select(x => new
        {
          x.SitecoreInfo,
          SharedSessionState = XmlExtensions.SelectSingleElement(x.SitecoreInfo.Configuration, "/configuration/sitecore/tracking/sharedSessionState")
        })
        .Where(x => x.SharedSessionState != null || // skip those that don't have shared session at all - like publishing instance
          ReturnFalse(_ => output.Debug($"Shared session state configuration is missing in {x.SitecoreInfo.InstanceName}")))
        .Select(x => new
        {
          x.SitecoreInfo,
          DefaultProviderName = x.SharedSessionState.GetAttribute("defaultProvider")
        })
        .Where(x => !string.IsNullOrWhiteSpace(x.DefaultProviderName) ||
          ReturnFalse(_ => errors.Add($"Shared session state default provider is not set - in {x.SitecoreInfo.InstanceName}")))
        .Select(x => new
        {
          x.SitecoreInfo,
          x.DefaultProviderName,
          Provider = x.SitecoreInfo.Configuration.SelectSingleElement($"/configuration/sitecore/tracking/sharedSessionState/providers/add[@name='{x.DefaultProviderName}']")
        })
        .Where(x => x.Provider != null ||
          ReturnFalse(_ => errors.Add($"Shared session state default provider {x.DefaultProviderName} cannot be found - in {x.SitecoreInfo.InstanceName}")))
        .ToDictionary(
          x => x.SitecoreInfo,
          x => x.Provider);

      // get a dictionary where key is instance's SitecoreInfo, and values are private session state providers' XML configuration 
      var privateProviders = data.Values // identifier is a combination of connection string and sessionType value e.g. shared or private or any custom string
        .Select(x => new
        {
          x.SitecoreInfo,
          SessionState = x.SitecoreInfo.Configuration.SelectSingleElement("/configuration/sessionState")
        })
        .Where(x => x.SessionState != null ||
          ReturnFalse(_ => output.Debug($"Private session state configuration is missing in {x.SitecoreInfo.InstanceName}")))
        .Select(x => new
        {
          x.SitecoreInfo,
          Mode = x.SessionState.GetAttribute("mode").EmptyToNull() ?? "InProc" // if not specified, InProc is used by default
        })
        .Where(x => !x.Mode.Equals("InProc", StringComparison.OrdinalIgnoreCase) ||
          ReturnFalse(_ => output.Debug($"Private session state configuration is InProc in {x.SitecoreInfo.InstanceName}")))
        .Select(x => new
        {
          x.SitecoreInfo,
          CustomProviderName = x.SitecoreInfo.Configuration.SelectSingleElement("/configuration/sessionState").GetAttribute("customProvider")
        })
        .Where(x => !string.IsNullOrWhiteSpace(x.CustomProviderName) ||
          ReturnFalse(_ => errors.Add($"Private session state custom provider is not set - in {x.SitecoreInfo.InstanceName}")))
        .Select(x => new
        {
          x.SitecoreInfo,
          x.CustomProviderName,
          Provider = x.SitecoreInfo.Configuration.SelectSingleElement($"/configuration/sessionState/providers/add[@name='{x.CustomProviderName}']")
        })
        .Where(x => x.Provider != null ||
          ReturnFalse(_ => errors.Add($"Private session state custom provider {x.CustomProviderName} cannot be found - in {x.SitecoreInfo.InstanceName}")))
        .ToDictionary(
          x => x.SitecoreInfo,
          x => x.Provider);

      if (errors.Any())
      {
        output.CannotRun("There are errors in configuration files that prevent test run. " + new BulletedList(errors));

        return;
      }

      var sharedSessionIdentifiers = GetIdentifiers(sharedProviders);
      var privateSessionIdentifiers = GetIdentifiers(privateProviders);

      // shared and private identifiers must not intersect
      var done = new List<string>();
      foreach (var privateSessionIdentifier in privateSessionIdentifiers)
      {
        var privateId = privateSessionIdentifier.Key;
        if (done.Contains(privateId))
        {
          continue;
        }

        foreach (var sharedSessionIdentifier in sharedSessionIdentifiers)
        {
          var sharedId = sharedSessionIdentifier.Key;
          if (done.Contains(sharedId))
          {
            continue;
          }

          var sharedInstancesNames = sharedSessionIdentifier.Value.Select(x => x.InstanceName);
          var privateInstancesNames = privateSessionIdentifier.Value.Select(x => x.InstanceName);
          if (string.Equals(privateId, sharedId, StringComparison.OrdinalIgnoreCase))
          {
            output.Error(GetMessage(privateId, privateInstancesNames, sharedInstancesNames));

            done.Add(privateId);
          }
        }
      }
    }

    protected ShortMessage GetMessage(string privateId, IEnumerable<string> privateInstancesNames, IEnumerable<string> sharedInstancesNames)
    {
      var message = new ShortMessage(
        new Text($"Shared and Private sessions interference is detected: "),
        new Code(privateId),
        new Text(" is used both for private and for shared session state providers."),
        new BulletedList(
          new Container(
            new Text("Private:"),
            new BulletedList(privateInstancesNames)
          ),
          new Container(
            new Text("Shared:"),
            new BulletedList(sharedInstancesNames))));
      return message;
    }

    private static Map<ISitecoreInformationContext[]> GetIdentifiers([NotNull] Dictionary<ISitecoreInformationContext, XmlElement> sha)
    {
      return sha
        .Select(x => new
        {
          SitecoreInfo = x.Key,
          DefaultProvider = x.Value,
        })
        .Select(x => new
        {
          x.SitecoreInfo,
          ConnectionStringName = x.DefaultProvider.GetAttribute("connectionString"),
          SessionTypeValue = x.DefaultProvider.GetAttribute("sessionType")
        })
        .Select(x => new
        {
          x.SitecoreInfo,
          x.SessionTypeValue,
          ConnectionStringValue = x.SitecoreInfo.GetConnectionString(x.ConnectionStringName)
        })
        .Select(x => new
        {
          x.SitecoreInfo,
          SessionIdentifier = x.ConnectionStringValue + "#" + x.SessionTypeValue
        })
        .GroupBy(x => x.SessionIdentifier)
        .ToMap(x => x.Key, x => x.ToArray(z => z.SitecoreInfo));
    }

    private bool ReturnFalse([NotNull] Action<Null> action)
    {
      action(null);

      return false;
    }
  }
}