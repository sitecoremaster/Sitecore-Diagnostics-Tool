namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Schema
{
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Newtonsoft.Json;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Diagnostics.SqlServer;

  public sealed class SqlDatabaseSchema : Sitecore.Diagnostics.Database.Schema.SqlDatabaseSchema
  {
    public SqlDatabaseSchema([NotNull] Database database, [NotNull] string databaseName)
    {
      Assert.ArgumentNotNull(database, nameof(database));
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));

      var tables = database
        .Tables
        .ToDictionary(x => x.Name, x => new Table(x) as Sitecore.Diagnostics.Database.Schema.Table);

      var storedProcedures = database
        .StoredProcedures
        .ToDictionary(x => x.Name, x => new StoredProcedure(x) as Sitecore.Diagnostics.Database.Schema.StoredProcedure);

      Name = databaseName;
      Tables = tables;
      StoredProcedures = storedProcedures;
    }

    public SqlDatabaseSchema(string jsonSchema)
    {
      var schema = JsonConvert.DeserializeObject<Schema>(jsonSchema, new JsonSerializerSettings
      {
        NullValueHandling = NullValueHandling.Ignore
      });
      Name = schema.Name;
      Tables = schema.Tables;
      StoredProcedures = schema.StoredProcedures;
    }

    internal class Schema
    {
      [NotNull]
      public string Name { get; set; }

      [NotNull]
      public Dictionary<string, Sitecore.Diagnostics.Database.Schema.Table> Tables { get; set; }

      [NotNull]
      public Dictionary<string, Sitecore.Diagnostics.Database.Schema.StoredProcedure> StoredProcedures { get; set; }
    }
  }
}