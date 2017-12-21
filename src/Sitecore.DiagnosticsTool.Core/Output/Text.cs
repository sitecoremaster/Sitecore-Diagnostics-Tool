namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Net;
  using System.Text;

  using JetBrains.Annotations;

  public class Text : MessagePart
  {
    [NotNull]
    public string Value { get; }

    public Text([NotNull] string text)
    {
      Value = text;
    }

    public static implicit operator Text([NotNull] string value)
    {
      return new Text(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.Append(WebUtility.HtmlEncode(Value));
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      builder.Append(Value.Replace("\r\n", "\n").Replace("\n", "\r\n" + shift));
    }
  }
}