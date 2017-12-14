namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System.Diagnostics;
  using System.Xml;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Extensions;

  [DebuggerDisplay("{" + nameof(FilePath) + "}")]
  public class ConfigurationFile
  {
    [CanBeNull]
    private XmlDocument _Configuration;

    [NotNull]
    public string FilePath { get; }

    [NotNull]
    public string RawText { get; }

    public ConfigurationFile([NotNull] string filePath, [NotNull] string rawText)
    {
      Assert.ArgumentNotNullOrEmpty(filePath, nameof(filePath));

      FilePath = filePath;
      RawText = rawText;
    }

    [NotNull]
    public XmlDocument Configuration => _Configuration ?? (_Configuration = new XmlDocument().TryParse(RawText));
  }
}