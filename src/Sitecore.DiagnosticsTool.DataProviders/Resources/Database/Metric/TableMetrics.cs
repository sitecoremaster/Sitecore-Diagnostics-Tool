namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Metric
{
  using System.Collections.Generic;
  using System.Linq;

  using Newtonsoft.Json;

  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class TableMetrics : ITableMetrics
  {
    private Dictionary<string, IIndexHealthMetrics> _IndexesHealth;

    [JsonProperty("IndexesHealth")]
    public virtual Dictionary<string, IndexHealthMetrics> JsonIndexesHealth { get; set; }

    [JsonProperty("Name")]
    public virtual string Name { get; set; }

    public virtual long RowCount { get; set; }

    public virtual double PhysicalSize { get; set; }

    [JsonIgnore]
    public virtual Dictionary<string, IIndexHealthMetrics> IndexesHealth
    {
      get
      {
        if (_IndexesHealth != null)
        {
          return _IndexesHealth;
        }

        lock (this)
        {
          if (_IndexesHealth != null)
          {
            return _IndexesHealth;
          }

          var value = JsonIndexesHealth?.ToDictionary(x => x.Key, x => (IIndexHealthMetrics)x.Value);
          foreach (var pair in value)
          {
            var index = (IndexHealthMetrics)pair.Value;
            index.Name = pair.Key;
          }

          _IndexesHealth = value;
        }

        return _IndexesHealth;
      }
    }
  }
}