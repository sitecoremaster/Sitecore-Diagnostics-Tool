namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper
{
  using System.Collections.Specialized;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Helpers.XPath;

  public static class XmlExtensions
  {
    [NotNull]
    public static XmlDocument Create([NotNull] this XmlDocument that, [NotNull] string xpath, [NotNull] NameValueCollection attributes, string contents = null)
    {
      Assert.ArgumentNotNull(that, nameof(that));
      Assert.ArgumentNotNull(xpath, nameof(xpath));
      Assert.ArgumentNotNull(attributes, nameof(attributes));

      Add(that, that, xpath, attributes, contents);

      return that;
    }

    [NotNull]
    public static XmlDocument FromXml([NotNull] this XmlDocument that, [NotNull] string xml)
    {
      Assert.ArgumentNotNull(that, nameof(that));
      Assert.ArgumentNotNull(xml, nameof(xml));

      that.LoadXml(xml);

      return that;
    }

    public static void Add([NotNull] this XmlDocument that, [NotNull] XmlNode parent, [NotNull] string xpath, [NotNull] NameValueCollection attributes, string contents = null)
    {
      Assert.ArgumentNotNull(that, nameof(that));
      Assert.ArgumentNotNull(parent, nameof(parent));
      Assert.ArgumentNotNull(xpath, nameof(xpath));
      Assert.ArgumentNotNull(attributes, nameof(attributes));

      foreach (var xpathSegment in XPathHelper.GetNodeSegments(xpath))
      {
        var element = that.CreateElement(xpathSegment.Name);

        foreach (var keyValuePair in xpathSegment.Attributes)
        {
          var key = keyValuePair.Key;
          if (key != null)
          {
            element.SetAttribute(key, keyValuePair.Value ?? string.Empty);
          }
        }

        parent.AppendChild(element);
        parent = element;
      }

      if (contents != null)
      {
        parent.InnerXml = contents;
      }

      foreach (string key in attributes.Keys)
      {
        var att = that.CreateAttribute(key);
        att.Value = attributes[key];
        parent.Attributes.Append(att);
      }
    }

    /// <summary>
    ///   Creates a XML document with one-in-one nodes with specified attributes via xpath (axes are not supported).
    /// </summary>
    [NotNull]
    public static XmlDocument Create([NotNull] this XmlDocument that, [NotNull] string xpath, string contents = null)
    {
      Assert.ArgumentNotNull(that, nameof(that));
      Assert.ArgumentNotNull(xpath, nameof(xpath));

      var parent = that as XmlNode;
      Add(that, parent, xpath, contents);

      return that;
    }

    /// <summary>
    ///   Creates a XML element with one-in-one nodes with specified attributes via xpath (axes are not supported) and appends
    ///   it to the specified parent.
    /// </summary>
    [NotNull]
    public static XmlDocument Add([NotNull] this XmlDocument that, [NotNull] string parentXPath, [NotNull] string xpath, string contents = null)
    {
      Assert.ArgumentNotNull(that, nameof(that));
      Assert.ArgumentNotNull(parentXPath, nameof(parentXPath));
      Assert.ArgumentNotNull(xpath, nameof(xpath));

      var parent = that.SelectSingleNode(parentXPath);
      Assert.IsNotNull(parent, "parent");

      return Add(that, parent, xpath, contents);
    }

    /// <summary>
    ///   Creates a XML element with one-in-one nodes with specified attributes via xpath (axes are not supported) and appends
    ///   it to the specified parent.
    /// </summary>
    [NotNull]
    public static XmlDocument Add([NotNull] this XmlDocument that, [NotNull] XmlNode parent, [NotNull] string xpath, string contents = null)
    {
      Assert.ArgumentNotNull(that, nameof(that));
      Assert.ArgumentNotNull(parent, nameof(parent));
      Assert.ArgumentNotNull(xpath, nameof(xpath));

      var nodeSegments = XPathHelper.GetNodeSegments(xpath);
      foreach (var nodeSegment in nodeSegments)
      {
        var node = that.CreateElement(nodeSegment.Name);
        foreach (var attribute in nodeSegment.Attributes)
        {
          var attributeName = attribute.Key;
          if (attributeName == null)
          {
            continue;
          }

          node.SetAttribute(attributeName, attribute.Value ?? string.Empty);
        }

        parent.AppendChild(node);
        parent = node;
      }

      if (contents != null)
      {
        parent.InnerXml = contents;
      }

      return that;
    }
  }
}