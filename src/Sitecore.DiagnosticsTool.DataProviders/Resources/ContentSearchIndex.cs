namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources
{
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;

  public class ContentSearchIndex : IContentSearchIndex
  {
    public ContentSearchIndex([NotNull] XmlElement indexConfiguration)
    {
      Assert.ArgumentNotNull(indexConfiguration);

      var id = indexConfiguration.GetAttribute("id");
      Assert.IsNotNullOrEmpty(id, nameof(id));

      var type = TypeRef.Parse(indexConfiguration.GetAttribute("type"));
      Assert.IsNotNull(type, nameof(type));

      Id = id;
      Type = type;
      SearchProvider = ParseSearchProvider(type);
      Configuration = indexConfiguration;
    }

    public string Id { get; }

    public TypeRef Type { get; }

    public SearchProvider SearchProvider { get; }

    public XmlElement Configuration { get; }

    private static SearchProvider ParseSearchProvider([NotNull] TypeRef type)
    {
      if (type == TypeRef.Parse("Sitecore.ContentSearch.LuceneProvider.LuceneIndex, Sitecore.ContentSearch.LuceneProvider"))
      {
        return SearchProvider.Lucene;
      }

      if (type == TypeRef.Parse("Sitecore.ContentSearch.SolrProvider.SolrSearchIndex, Sitecore.ContentSearch.SolrProvider"))
      {
        return SearchProvider.Solr;
      }

      if (type == TypeRef.Parse("Sitecore.ContentSearch.Azure.CloudSearchProviderIndex, Sitecore.ContentSearch.Azure"))
      {
        return SearchProvider.AzureSearch;
      }

      return SearchProvider.Other;
    }
  }
}