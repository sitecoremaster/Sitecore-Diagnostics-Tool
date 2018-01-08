namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;
  using Sitecore.DiagnosticsTool.Tests.ECM.Helpers;

  public class CheckConnectionString : Test
  {
    [UsedImplicitly]
    public CheckConnectionString()
    {
    }

    public override string Name { get; } = "Presence of EXM-specific connection strings";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Ecm };

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 8;
    }

    protected override bool IsActual(IInstanceResourceContext data)
    {
      var exmMajor = EcmHelper.GetEcmVersion(data)?.Major;

      return exmMajor == 3 || exmMajor == 2;
    }

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var ecmVersion = EcmHelper.GetEcmVersion(data);
      if (ecmVersion == null)
      {
        return;
      }

      if (ecmVersion.Major == 3 && ecmVersion.Minor >= 3)
      {
        Error(data, output, "exm.web");
        Error(data, output, "EXM.CryptographicKey");
        Error(data, output, "EXM.AuthenticationKey");

        if (data.Databases.ConnectionStrings.ContainsKey("master"))
        {
          Error(data, output, "exm.master");
        }

        if (data.ServerRoles.Contains(ServerRole.ContentDelivery) &&
            !data.ServerRoles.Contains(ServerRole.ContentManagement))
        {
          Error(data, output, "EXM.InternalApiKey");
          Error(data, output, "EmailCampaignClientService");
        }

        return;
      }

      if (!data.ServerRoles.Contains(ServerRole.ContentManagement))
      {
        return;
      }

      if (ecmVersion.Major == 3 && ecmVersion.Minor >= 1)
      {
        if (!data.Databases.Sql.DatabaseNames.Contains("exm.dispatch"))
        {
          output.Error(GetErrorMessage("exm.dispatch"));
        }
      }
        
      if (ecmVersion.Major == 2 && ecmVersion.Minor == 2 || ecmVersion.Major == 3 && ecmVersion.Minor == 0)
      {
        if (!data.Databases.Mongo.DatabaseNames.Contains("ecm.dispatch"))
        {
          output.Error(GetErrorMessage("ecm.dispatch"));
        }
      }
    }

    private void Error(IInstanceResourceContext data, ITestOutputContext output, string name)
    {
      var value = data.Databases.ConnectionStrings.TryGetValue(name.ToLower());
      if (value == null)
      {
        output.Error(GetErrorMessage(name));
      }
    }
    
    [NotNull]
    public string GetErrorMessage([NotNull] string connectionString)
    {
      Assert.ArgumentNotNull(connectionString, nameof(connectionString));

      return $"The necessary '{connectionString}' connection string is missing. Please review the module installation guide for details.";
    }
  }
}