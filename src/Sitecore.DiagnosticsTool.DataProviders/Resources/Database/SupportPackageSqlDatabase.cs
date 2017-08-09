namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database
{
  using System;
  using System.IO;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Database.Schema;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Metric;

  public class SupportPackageSqlDatabase : ISqlDatabase
  {
    private Database _Metrics;
    private string RootPath { get; }
    private DatabaseSchema _Schema;

    public SupportPackageSqlDatabase([NotNull] string databaseName, [NotNull] string connectString, string rootPath)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectString, nameof(connectString));

      Name = databaseName;
      ConnectionString = connectString;
      Type = GetDatabaseType(databaseName);
      RootPath = rootPath;
    }

    public string Name { get; }
    public string ConnectionString { get; }
    public DatabaseType Type { get; }

    public DatabaseSchema Schema
    {
      get
      {
        try
        {
          if (_Schema != null)
          {
            return _Schema;
          }
          var jsonSchema = File.ReadAllText(Path.Combine(RootPath, "Databases", "Schemas", $"{Name}.json"));
          return _Schema = new Schema.DatabaseSchema(jsonSchema);
        }
        catch (Exception)
        {
          throw new DatabaseResourceNotAvailableException();
        }
      }
    }

    public IDatabaseMetrics Metrics
    {
      get
      {
        try
        {
          if (_Metrics != null)
          {
            return _Metrics;
          }
          var jsonMetrics = File.ReadAllText(Path.Combine(RootPath, "Databases", "Metrics", $"{Name}.json"));
          return _Metrics = JsonConvert.DeserializeObject<Database>(jsonMetrics);
        }
        catch (Exception)
        {
          throw new DatabaseResourceNotAvailableException();
        }
      }
    }

    public int CountRows(string tableName)
    {
      return (int)Metrics.Tables[tableName].RowCount;
    }

    public int CountRows(string tableName, string condition)
    {
      throw new DatabaseResourceNotAvailableException();
    }

    private DatabaseType GetDatabaseType(string databaseName)
    {
      switch (databaseName.ToLower())
      {
        case "core":
          return DatabaseType.Core;

        case "master":
        case "web":
          return DatabaseType.Content;

        case "reporting":
        case "reporting.secondary":
          return DatabaseType.Reporting;

        default:
          return DatabaseType.Undefined;
      }
    }
  }
}