namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System;
  using System.Text;
  using JetBrains.Annotations;

  public abstract class MessagePart
  {
    public abstract void ToHtml([NotNull] StringBuilder builder);

    public abstract void ToPlainText([NotNull] StringBuilder builder, string shift = null);

    [NotNull]
    public string ToString(OutputFormat format)
    {
      var builder = new StringBuilder();
      switch (format)
      {
        case OutputFormat.Text:
          ToPlainText(builder);
          break;

        case OutputFormat.Html:
          ToHtml(builder);
          break;

        default:
          throw new NotImplementedException();
      }

      return builder.ToString();
    }

    /// <summary>
    ///   Renders text in markdown format. Use overload to see other options.
    /// </summary>
    public sealed override string ToString()
    {
      return ToString(OutputFormat.Text);
    }
  }
}