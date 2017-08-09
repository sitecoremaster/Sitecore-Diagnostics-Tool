namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database
{
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class SupportPackageSqlDatabaseContext : ISqlDatabaseContext
  {
    private SupportPackageSqlDatabaseContext([NotNull] Dictionary<string, ISqlDatabase> databasesList)
    {
      Assert.IsNotNull(databasesList, "databasesList");

      Databases = databasesList;
    }

    public Dictionary<string, ISqlDatabase> Databases { get; set; }

    public string[] DatabaseNames => Databases.Keys.ToArray();

    public ISqlDatabase[] All => Databases.Values.ToArray();

    public ISqlDatabase this[string name]
    {
      get
      {
        ISqlDatabase value;
        Databases.TryGetValue(name, out value);

        return value;
      }
    }

    public static ISqlDatabaseContext Parse([NotNull] IReadOnlyDictionary<string, string> connectionStrings, string rootPath)
    {
      Assert.ArgumentNotNull(connectionStrings, nameof(connectionStrings));

      Log.Info($"Parsing {connectionStrings.Count} SQL connection strings");
      var databasesList = connectionStrings.ToDictionary(x => x.Key, x => new SupportPackageSqlDatabase(x.Key, x.Value, rootPath) as ISqlDatabase);

      return databasesList.Count > 0 ? new SupportPackageSqlDatabaseContext(databasesList) : null;
    }
  }
}