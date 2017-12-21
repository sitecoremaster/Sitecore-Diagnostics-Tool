namespace Sitecore.DiagnosticsTool.Tests.General.Health
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, log-based)
  [UsedImplicitly]
  public class InvalidWebResource : KbTest
  {
    public override string KbNumber => "999445";

    public override string KbName { get; } = "\"This is an invalid webresource request\" error message";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var ex = log.Exception;
        if (ex == null)
        {
          return;
        }

        if (ex.Message.Contains("This is an invalid webresource request.") && ex.StackTrace.Contains("System.Web.Handlers.AssemblyResourceLoader.System.Web.IHttpHandler.ProcessRequest(HttpContext context)"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}