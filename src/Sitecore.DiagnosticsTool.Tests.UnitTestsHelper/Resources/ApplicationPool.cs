namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System.Collections.Generic;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;

  public class ApplicationPool : IApplicationPool
  {
    public FrameworkVersion FrameworkVersion { get; set; }

    public string FrameworkVersionRaw { get; set; }

    public Identity Identity { get; set; }

    public bool PingingEnabled { get; set; }

    public FrameworkBitness FrameworkBitness { get; set; }

    public PipelineMode ManagedPipelineMode { get; set; }

    public string Name { get; set; }

    public IEnumerable<int> WorkerProcessesIds { get; set; }

    public State State { get; set; }

    public int MaxWorkerProcesses { get; set; }

    public bool LoadUserProfile { get; set; }
  }
}