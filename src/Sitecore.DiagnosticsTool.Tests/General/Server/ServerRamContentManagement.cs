// <summary>
//   http://issues.pssbuild1dk1.dk.sitecore.net/issue/SDT-61
// </summary>

namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  public class ServerRamContentManagement : Test
  {
    protected const int Recommended = 8;

    [NotNull]
    protected static readonly string Message = $"Web Server has less than {Recommended} GB of RAM. Read more in Installation Guide, section Sitecore Hosting Environment Requirements";

    public override string Name { get; } = "Web Server RAM amount";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.Performance };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var ramMemory = data.WebServer.Info.RamMemoryTotal;
      if (ramMemory.Gb < Recommended)
      {
        output.Warning(Message);
      }
    }
  }
}