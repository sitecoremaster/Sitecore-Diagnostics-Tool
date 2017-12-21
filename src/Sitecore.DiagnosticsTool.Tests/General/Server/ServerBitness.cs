// <summary>
//   http://issues.pssbuild1dk1.dk.sitecore.net/issue/SDT-79
// </summary>

namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.WebServer;
  using Sitecore.DiagnosticsTool.Core.Tests;

  [UsedImplicitly]
  public class ServerBitness : Test
  {
    public override string Name { get; } = "Web Server OS Bitness";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Performance };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      if (data.WebServer.Server.OperationSystemBitness != FrameworkBitness.x64)
      {
        output.Warning("The 32-bit Operation System is used, upgrade to 64-bit OS is recommended. Read more in Installation Guide, section Sitecore Hosting Environment Requirements");
      }
    }
  }
}