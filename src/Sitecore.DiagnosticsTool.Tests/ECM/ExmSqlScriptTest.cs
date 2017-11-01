namespace Sitecore.DiagnosticsTool.Tests.ECM
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ExmSqlScriptTest : Test
  {
    [NotNull]
    protected readonly string[] ProcedureNames =
    {
      "Add_VisitsByMessage",
      "Add_AutomationStatesStatisticsByMessage",
      "Add_AutomationStatesStatisticsByAbnMessage",
      "Add_LandingPages",
      "Add_AbnVisitsByMessage",
      "Ensure_LandingPageDetails"
    };

    [NotNull]
    protected readonly string[] TableNames =
    {
      "LandingPageDetails",
      "Fact_LandingPages",
      "Fact_VisitsByMessage",
      "Fact_AbnVisitsByMessage",
      "Fact_AutomationStatesStatisticsByMessage",
      "Fact_AutomationStatesStatisticsByAbnMessage"
    };

    public override string Name { get; } = "Check if EXM SQL script was run against reporting database";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Ecm};

    protected override bool IsActual(ISitecoreVersion sitecoreVersion)
    {
      return sitecoreVersion.Major >= 8;
    }

    protected override bool IsActual(IReadOnlyCollection<ServerRole> roles)
    {
      // this test is valid only for authoring or reporting instances
      return roles.Contains(ServerRole.ContentManagement) || roles.Contains(ServerRole.Reporting);
    }

    /// <inheritdoc />
    public override bool IsActual(IReadOnlyCollection<ServerRole> roles, ISitecoreVersion sitecoreVersion, ITestResourceContext data)
    {
      try
      {
        if (!data.SitecoreInfo.Assemblies.ContainsKey("Sitecore.EmailCampaign.dll".ToLower()))
        {
          return false;
        }
      }
      catch
      {
      }

      try
      {
        if (data.SitecoreInfo.Configuration.SelectSingleElement("/configuration/sitecore/TypeResolver") == null)
        {
          return false;
        }
      }
      catch
      {
      }

      return true;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var xdbEnabled = data.SitecoreInfo.IsAnalyticsEnabled;
      var name = "reporting";
      var reporting = data.Databases.Sql[name];
      if (reporting == null)
      {
        var message = $"The {name} connection string is not presented in the ConnectionStrings.config file";
        if (xdbEnabled)
        {
          output.Error(message);
        }
        else
        {
          output.Debug(message + ", but that's okay since xdb is disabled");
        }

        return;
      }

      var schema = reporting.Schema;

      var sb = new StringBuilder();

      // check tables
      foreach (var tableName in TableNames)
      {
        if (!schema.Tables.ContainsKey(tableName))
        {
          sb.AppendFormat("\r\n- {0}.Tables.dbo.{1}", name, tableName);
        }
      }

      foreach (var procedureName in ProcedureNames)
      {
        if (!schema.StoredProcedures.ContainsKey(procedureName))
        {
          sb.AppendFormat("\r\n- {0}.Programmability.Stored Procedures.dbo.{1}", name, procedureName);
        }
      }

      if (sb.Length > 0)
      {
        var message = $"One or several objects are missing in the reporting database. This may happen if EXM SQL script was not run or ended with error. Please refer to EXM installation guide for more details.:{sb}";
        if (xdbEnabled)
        {
          output.Error(message);
        }
        else
        {
          output.Debug(message);
        }
      }
    }
  }
}