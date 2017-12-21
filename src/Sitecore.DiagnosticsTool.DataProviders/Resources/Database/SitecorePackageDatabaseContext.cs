namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database
{
  using System;
  using System.Collections.Generic;
  using System.Data.SqlClient;
  using System.Linq;
  using System.Xml;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  public class SitecorePackageDatabaseContext : ResourceBase, IDatabaseContext
  {
    protected IMongoDatabaseContext MongoContext { get; }

    protected ISqlDatabaseContext SqlContext { get; }

    public IReadOnlyDictionary<string, string> ConnectionStrings { get; }

    protected SitecorePackageDatabaseContext(ISqlDatabaseContext sqlDatabaseContext,
      IMongoDatabaseContext mongoDatabaseContext, IReadOnlyDictionary<string, string> connectionStrings)
    {
      SqlContext = sqlDatabaseContext;
      MongoContext = mongoDatabaseContext;
      ConnectionStrings = connectionStrings;
    }

    protected override ResourceType ResourceType => ResourceType.Database;

    public ISqlDatabaseContext Sql => AssertResource(SqlContext, "SQL Server");

    public IMongoDatabaseContext Mongo => AssertResource(MongoContext, "Mongo");

    public static SitecorePackageDatabaseContext TryParse(string root, SitecoreInformationContext info)
    {
      Assert.ArgumentNotNull(root, nameof(root));
      Assert.ArgumentNotNull(info, nameof(info));

      var connectionStringsNode = info.Configuration.SelectElements("/configuration/connectionStrings").SingleOrDefault();
      if (connectionStringsNode == null)
      {
        Log.Error("no connection strings node");
        return null;
      }

      var connectionStringsList = connectionStringsNode.GetElementsByTagName("add");
      if (connectionStringsList.Count <= 0)
      {
        Log.Error("no connection strings");
        return null;
      }

      var sqlConnectionStrings = new Dictionary<string, string>();
      var mongoConnectionStrings = new Dictionary<string, string>();
      foreach (var connectionStringElement in connectionStringsList.OfType<XmlElement>())
      {
        var connectionString = connectionStringElement.GetAttribute("connectionString");
        var name = connectionStringElement.GetAttribute("name");

        Log.Info($"Parsing connection string: {name}, value: {connectionString}");
        if (string.IsNullOrWhiteSpace(name))
        {
          continue;
        }

        if (connectionString.StartsWith("mongodb", StringComparison.InvariantCultureIgnoreCase))
        {
          mongoConnectionStrings.Add(name, connectionString);
        }
        else
        {
          try
          {
            new SqlConnectionStringBuilder(connectionString);
          }
          catch (Exception ex)
          {
            if (ex is FormatException || ex is ArgumentException)
            {
              Log.Info(ex, $"Unknown format of connection string: {connectionString}");
            }
            else
            {
              Log.Error(ex, $"Unknown issue occurred during parsing connection string: {connectionString}");
            }

            continue;
          }

          sqlConnectionStrings.Add(name, connectionString);
        }
      }

      var connectionStrings = new Dictionary<string, string>();
      foreach (var connectionStringElement in connectionStringsList.OfType<XmlElement>())
      {
        var name = connectionStringElement.GetAttribute("name");
        var connectionString = connectionStringElement.GetAttribute("connectionString");

        connectionStrings.Add(name.ToLower(), connectionString);
      }

      return new SitecorePackageDatabaseContext(SupportPackageSqlDatabaseContext.Parse(sqlConnectionStrings, root), MongoDatabaseContext.Parse(mongoConnectionStrings), connectionStrings);
    }
  }
}