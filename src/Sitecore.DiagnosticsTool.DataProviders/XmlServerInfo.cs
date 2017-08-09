namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Xml.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class XmlServerInfo : IServerInfo
  {
    private int? Cores { get; }

    private List<FrameworkVersion> _FrameworkVersions { get; }

    private string Name { get; }

    private Size? Ram { get; }
    private string Version { get; }

    public XmlServerInfo([NotNull] XContainer hardwareInfo)
    {
      Assert.ArgumentNotNull(hardwareInfo, nameof(hardwareInfo));

      Version = hardwareInfo.Document.Element("ServerInfo")?.Element("Software")?.Element("OS")?.Element("Version")?.Value;
      Name = hardwareInfo.Document.Element("ServerInfo")?.Element("Software")?.Element("MachineName")?.Value;
      var ramTotal = hardwareInfo.Document.Element("ServerInfo")?.Element("Hardware")?.Element("Ram")?.Element("Total")?.Value;
      if (!string.IsNullOrEmpty(ramTotal))
      {
        Ram = Size.FromBytes(ulong.Parse(ramTotal));
      }

      var processorCount = hardwareInfo.Document.Element("ServerInfo")?.Element("Hardware")?.Element("ProcessorCount")?.Value;
      if (!string.IsNullOrEmpty(processorCount))
      {
        Cores = int.Parse(processorCount);
      }

      var frameworkVersionsElements = hardwareInfo.Document.Element("ServerInfo")?.Element("Software")?.Element("FrameworkVersions")?.Elements();

      if (frameworkVersionsElements == null)
      {
        return;
      }
      if (_FrameworkVersions == null)
      {
        _FrameworkVersions = new List<FrameworkVersion>();
      }

      foreach (var frameworkVersion in frameworkVersionsElements)
      {
        FrameworkVersion v;
        if (Enum.TryParse(frameworkVersion?.Value, true, out v))
        {
          _FrameworkVersions.Add(v);
        }
      }
    }

    public FrameworkBitness OperationSystemBitness => Throw<FrameworkBitness>();

    public string OperationSystemVersion => Version ?? Throw<string>();

    public string MachineName => Name ?? Throw<string>();

    public Size RamMemoryTotal => Ram ?? Throw<Size>();

    public int CpuCoresCount => Cores ?? Throw<int>();

    public IReadOnlyList<FrameworkVersion> FrameworkVersions => _FrameworkVersions ?? Throw<IReadOnlyList<FrameworkVersion>>();

    public FileVersionInfo IisVersion => Throw<FileVersionInfo>();

    [NotNull]
    private T Throw<T>()
    {
      throw new WebServerResourceNotAvailableException("ServerInfo");
    }
  }
}