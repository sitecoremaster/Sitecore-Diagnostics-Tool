namespace Sitecore.DiagnosticsTool.Tests.Analytics
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-12, looks reasonable)
  [UsedImplicitly]
  public class ReportingSecondaryConnectionString : Test
  {
    protected const string ConnectionString = "reporting.secondary";

    protected const string ErrorMessage = "Connection string reporting.secondary is present. The reporting.secondary database is populated even if the rebuild is not running. It is recommended to comment out the connection string if the rebuild process is not running to reduce load on SQL server.";

    public override string Name { get; } = "Connection string reporting.secondary is present";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.Analytics};

    protected override bool IsActual(ITestResourceContext data)
    {
      return data.SitecoreInfo.IsAnalyticsEnabled;
    }

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      var reportingSecondary = data.Databases.Sql.DatabaseNames.Contains(ConnectionString);
      if (reportingSecondary)
      {
        output.Warning(ErrorMessage);
      }
    }
  }
}