namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Text;
  using JetBrains.Annotations;

  public class CodeBlock : Text
  {
    public CodeBlock([NotNull] string code)
      : base(code)
    {
    }

    public static implicit operator CodeBlock([NotNull] string value)
    {
      return new CodeBlock(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.AppendLine($"<pre>");

      base.ToHtml(builder);

      builder.AppendLine($"</pre>");
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      builder.AppendLine();
      builder.Append(shift);

      builder.AppendLine($"```");
      builder.Append(shift);

      base.ToPlainText(builder, shift);

      builder.AppendLine();
      builder.Append(shift);

      builder.AppendLine($"```");
      builder.Append(shift);
    }
  }
}