namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Text;
  using JetBrains.Annotations;

  public class Code : Text
  {
    public Code([NotNull] string code)
      : base(code)
    {
    }

    public static implicit operator Code([NotNull] string value)
    {
      return new Code(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.AppendLine($"<code>");

      base.ToHtml(builder);

      builder.AppendLine($"</code>");
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      builder.Append($"`");

      base.ToPlainText(builder, shift);

      builder.Append($"`");
    }
  }
}