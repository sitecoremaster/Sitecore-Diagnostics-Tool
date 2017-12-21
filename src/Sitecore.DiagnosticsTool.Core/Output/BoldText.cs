namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Text;

  using JetBrains.Annotations;

  public class BoldText : Text
  {
    public BoldText([NotNull] string text)
      : base(text)
    {
    }

    public static implicit operator BoldText([NotNull] string value)
    {
      return new BoldText(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.Append($"<b>");

      base.ToHtml(builder);

      builder.Append("</b>");
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      builder.Append($"**");

      base.ToPlainText(builder); // no shift

      builder.Append("**");
    }
  }
}