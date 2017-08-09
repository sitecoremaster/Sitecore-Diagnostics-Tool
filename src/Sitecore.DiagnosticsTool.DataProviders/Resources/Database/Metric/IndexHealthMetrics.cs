namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database.Metric
{
  using Newtonsoft.Json;
  using Sitecore.DiagnosticsTool.Core.Resources.Database;

  public class IndexHealthMetrics : IIndexHealthMetrics
  {
    [JsonProperty("Name")]
    public virtual string Name { get; set; }

    public virtual double AverageFragmentationInPercent { get; set; }

    public virtual long PageCount { get; set; }
  }
}