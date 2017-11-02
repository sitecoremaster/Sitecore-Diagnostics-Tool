namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Database.Schema;
  using Sitecore.Diagnostics.InfoService.Client.Model.Defaults;
  using Sitecore.Diagnostics.InfoService.Client.Model.Defaults.DatabaseExtensions;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  public class ReleaseDefaultSqlDatabase : IReleaseDefaultSqlDatabase
  {
    [NotNull]
    private IDefaultSqlDatabaseInfo Database { get; }

    public ReleaseDefaultSqlDatabase([NotNull] string name, [NotNull] IDefaultSqlDatabaseInfo database)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(database, nameof(database));

      Name = name;
      Database = database;
    }

    public string Name { get; }

    public SqlDatabaseSchema Schema => Database.SqlSchema();
  }
}