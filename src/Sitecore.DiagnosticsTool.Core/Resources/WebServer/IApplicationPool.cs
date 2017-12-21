namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  public interface IApplicationPool
  {
    /// <summary>
    ///   .NET Framework version (can be somewhat inaccurate)
    /// </summary>
    FrameworkVersion FrameworkVersion { get; }

    /// <summary>
    ///   .NET Framework version (exact, retrieved by Environment.Version, looks like "4.0.30319.17626")
    /// </summary>
    [NotNull]
    string FrameworkVersionRaw { get; }

    /// <summary>
    ///   The user account that is used for IIS application pool.
    /// </summary>
    [NotNull]
    Identity Identity { get; }

    /// <summary>
    ///   Pinging Enabled application pool property.
    /// </summary>
    bool PingingEnabled { get; }

    /// <summary>
    ///   .NET Framework bitness (x86 or x64).
    /// </summary>
    FrameworkBitness FrameworkBitness { get; }

    /// <summary>
    ///   Managed pipeline mode (Classic or Integrated).
    /// </summary>
    PipelineMode ManagedPipelineMode { get; }

    /// <summary>
    ///   The name of application pool.
    /// </summary>
    [NotNull]
    string Name { get; }

    /// <summary>
    ///   The IDs of current worker processes associated with the application pool.
    /// </summary>
    [NotNull]
    IEnumerable<int> WorkerProcessesIds { get; }

    /// <summary>
    ///   The maximum number of worker processes allowed to have for application pool.
    /// </summary>
    int MaxWorkerProcesses { get; }

    /// <summary>
    ///   Whether Load User Profile is set to true for application pool or not.
    /// </summary>
    bool LoadUserProfile { get; }
  }
}