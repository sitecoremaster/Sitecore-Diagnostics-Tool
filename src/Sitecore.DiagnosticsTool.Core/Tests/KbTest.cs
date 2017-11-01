namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  /// <summary>
  ///   Abstract class that is designed for knowledgebase-related tests.
  /// </summary>
  public abstract class KbTest : Test
  {
    protected const string ErrorFormat = "The known issue #{0} can be potentially applicable to the solution";

    protected const string ErrorFormatWithMessage = "The known issue #{0} ({2}) can be potentially applicable to the solution. {1}";

    public sealed override string Name => "KB #" + KbNumber + " (" + KbName + ")";

    /// <summary>
    ///   Link to the knowledgebase article.
    /// </summary>
    [NotNull]
    public Uri Link => new Uri("https://kb.sitecore.net/articles/" + KbNumber);

    [NotNull]
    public abstract string KbNumber { get; }

    [NotNull]
    public abstract string KbName { get; }

    /// <summary>
    ///   The method for claiming KB issue is applicable to the solution.
    /// </summary>
    protected void Report([NotNull] ITestOutputContext output, string message = null)
    {
      Assert.ArgumentNotNull(output, nameof(output));

      if (!string.IsNullOrEmpty(message))
      {
        if (char.IsLower(message[0]))
        {
          message = message.Substring(0, 1).ToUpper() + message.Substring(1);
        }

        output.Warning(string.Format(ErrorFormatWithMessage, KbNumber, message, KbName), url: Link);
      }
      else
      {
        output.Warning(string.Format(ErrorFormat, KbNumber), url: Link);
      }
    }
  }
}