namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Xml;
  using Sitecore.Diagnostics.Objects;

  public interface IContentSearchIndex
  {
    /// <summary>
    ///   Name of ContentSearch index.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///   .NET Type of ContentSearch index.
    /// </summary>
    TypeRef Type { get; }

    /// <summary>
    ///   Type of ContentSearch index implementation.
    /// </summary>
    SearchProvider SearchProvider { get; }

    /// <summary>
    ///   XML configuration element.
    /// </summary>
    XmlElement Configuration { get; }
  }
}