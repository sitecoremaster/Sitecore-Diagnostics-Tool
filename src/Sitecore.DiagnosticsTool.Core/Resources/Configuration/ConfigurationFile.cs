namespace Sitecore.DiagnosticsTool.Core.Resources.Configuration
{
  using System;
  using System.IO;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Extensions;

  public class ConfigurationFile
  {
    [NotNull]
    public string FilePath { get; }

    private XmlDocument _Configuration;

    private string _RawText;

    public ConfigurationFile([NotNull] string filePath)
    {
      Assert.ArgumentNotNullOrEmpty(filePath, nameof(filePath));

      FilePath = filePath;
    }

    /// <summary>
    ///   The raw text stored in the file.
    /// </summary>
    [NotNull]
    [PublicAPI]
    public string RawText
    {
      get
      {
        return _RawText ?? (_RawText = _Configuration.With(x => x.OuterXml) ?? File.ReadAllText(FilePath));
      }
    }

    /// <summary>
    ///   The XmlDocument pared from the file's raw text.
    /// </summary>
    /// <exception cref="InvalidOperationException">Failed to load XML file: {0}</exception>
    [NotNull]
    [PublicAPI]
    public XmlDocument Configuration => _Configuration ?? (_Configuration = new XmlDocument().TryLoadFile(FilePath)).IsNotNull(nameof(_Configuration));
  }
}