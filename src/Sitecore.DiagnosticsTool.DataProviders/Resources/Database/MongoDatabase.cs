namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class MongoDatabase : IMongoDatabase
  {
    public MongoDatabase([NotNull] string databaseName, [NotNull] string connectString)
    {
      Assert.ArgumentNotNull(databaseName, nameof(databaseName));
      Assert.ArgumentNotNull(connectString, nameof(connectString));

      Name = databaseName;
      ConnectionString = connectString;
    }

    public string Name { get; }

    public string ConnectionString { get; }
  }
}