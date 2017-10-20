namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  public enum SearchProvider
  {
    Lucene = 1 << 1,
    Solr = 1 << 2,
    AzureSearch = 1 << 3,
    Other = 1 << 9
  }
}