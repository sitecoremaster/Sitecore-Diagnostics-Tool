namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class MongoDatabaseContext : IMongoDatabaseContext
  {
    [NotNull]
    protected readonly Dictionary<string, IMongoDatabase> Databases;

    private MongoDatabaseContext([NotNull] Dictionary<string, IMongoDatabase> databasesList)
    {
      Assert.IsNotNull(databasesList, "databasesList");

      Databases = databasesList;
    }

    public string[] DatabaseNames => Databases.Keys.ToArray();

    public IMongoDatabase[] All
    {
      get
      {
        return Databases
          .Select(x => new MongoDatabase(x.Key, x.Value.ConnectionString))
          .ToArray();
      }
    }

    public IMongoDatabase this[string name]
    {
      get
      {
        IMongoDatabase value;
        Databases.TryGetValue(name, out value);

        return value;
      }
    }

    public static IMongoDatabaseContext Parse([NotNull] IReadOnlyDictionary<string, string> connectionStrings)
    {
      Assert.ArgumentNotNull(connectionStrings, nameof(connectionStrings));

      Log.Info($"Parsing {connectionStrings.Count} mongo connection strings");
      var databases = connectionStrings.ToDictionary(x => x.Key, x => new MongoDatabase(x.Key, x.Value) as IMongoDatabase);

      return databases.Count > 0 ? new MongoDatabaseContext(databases) : null;
    }
  }
}