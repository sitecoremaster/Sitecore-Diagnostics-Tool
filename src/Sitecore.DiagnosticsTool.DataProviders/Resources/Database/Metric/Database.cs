namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Metric
{
  using System.Collections.Generic;
  using System.Linq;

  using Newtonsoft.Json;

  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class Database : IDatabaseMetrics
  {
    private Dictionary<string, ITableMetrics> _Tables;

    [JsonProperty("Tables")]
    public virtual Dictionary<string, TableMetrics> JsonTables { get; set; }

    [JsonProperty("Name")]
    public virtual string Name { get; set; }

    [JsonIgnore]
    public virtual Dictionary<string, ITableMetrics> Tables
    {
      get
      {
        if (_Tables != null)
        {
          return _Tables;
        }

        lock (this)
        {
          if (_Tables != null)
          {
            return _Tables;
          }

          var value = JsonTables.ToDictionary(x => x.Key, x => (ITableMetrics)x.Value);
          foreach (var table in value)
          {
            var tableMetrics = (TableMetrics)table.Value;
            tableMetrics.Name = table.Key;
          }

          _Tables = value;
        }

        return _Tables;
      }
    }
  }
}