namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class MaximumWorkerProcesses : Test
  {
    public override string Name { get; } = "Maximum worker processes in application pool";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var pool = data.WebServer.CurrentSite.ApplicationPool;
      if (pool.MaxWorkerProcesses > 1)
      {
        output.Error(GetErrorMessage(pool));
      }
    }

    [NotNull]
    protected static string GetErrorMessage([NotNull] IApplicationPool pool)
    {
      Assert.ArgumentNotNull(pool, nameof(pool));

      return $"The sites application pool (\"{pool.Name}\") is configured to allow using more than 1 worker process, which is not supported by Sitecore. Read more in Installation Guide, section Maximum Number of Worker Processes";
    }
  }
}