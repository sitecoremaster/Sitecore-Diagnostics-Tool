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
  public class MediaCachingError : KbTest
  {
    public override string KbNumber => "843582";

    public override string KbName { get; } = "Errors in log files while caching media items";

    public override IEnumerable<Category> Categories { get; } = new[] { Category.General };

    public override void Process(IInstanceResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var logs = data.Logs.GetSitecoreLogEntries(LogLevel.All);
      foreach (var log in logs)
      {
        var message = log.Message;
        var ex = log.Exception;
        if (ex == null)
        {
          return;
        }

        if (message.Contains("OpenFileStream failed: ") && ex.Message.Contains("The process cannot access the file ") && ex.StackTrace.Contains("System.IO.FileStream.Init(String path, FileMode mode, FileAccess access, Int32 rights, Boolean useRights, FileShare share, Int32 bufferSize, FileOptions options, SECURITY_ATTRIBUTES secAttrs, String msgPath, Boolean bFromProxy, Boolean useLongPath, Boolean checkHost)"))
        {
          Report(output);
          return;
        }
      }
    }
  }
}