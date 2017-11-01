namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System.Collections.Generic;
  using System.Diagnostics;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;

  public interface IServerInfo
  {
    /// <summary>
    ///   The operating system bitness (x86 or x64).
    /// </summary>
    FrameworkBitness OperationSystemBitness { get; }

    /// <summary>
    ///   The operating system version.
    /// </summary>
    [NotNull]
    string OperationSystemVersion { get; }

    /// <summary>
    ///   The machine name.
    /// </summary>
    [NotNull]
    string MachineName { get; }

    /// <summary>
    ///   The total amount of physical memory on the server. Note, the value can be up to 10% less than actual.
    /// </summary>
    Size RamMemoryTotal { get; }

    /// <summary>
    ///   The count of CPU cores on the server.
    /// </summary>
    int CpuCoresCount { get; }

    /// <summary>
    ///   The list of installed .NET Framework versions.
    /// </summary>
    IReadOnlyList<FrameworkVersion> FrameworkVersions { get; }

    /// <summary>
    ///   The version of installed IIS.
    /// </summary>
    FileVersionInfo IisVersion { get; }
  }
}