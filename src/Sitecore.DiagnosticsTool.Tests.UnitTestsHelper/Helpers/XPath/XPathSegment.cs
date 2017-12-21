namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Helpers.XPath
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public class XPathSegment
  {
    [NotNull]
    public readonly IEnumerable<KeyValuePair<string, string>> Attributes;

    [NotNull]
    public string Name { get; }

    public XPathSegment([NotNull] string name)
    {
      Assert.ArgumentNotNull(name, nameof(name));

      Name = name;
      Attributes = new KeyValuePair<string, string>[0];
    }

    public XPathSegment([NotNull] string name, [NotNull] IEnumerable<KeyValuePair<string, string>> attributes)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(attributes, nameof(attributes));

      Name = name;
      Attributes = attributes;
    }
  }
}