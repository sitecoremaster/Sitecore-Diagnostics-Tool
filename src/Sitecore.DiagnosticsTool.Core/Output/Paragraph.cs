namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Text;

  using JetBrains.Annotations;

  public class Paragraph : Container
  {
    public Paragraph([CanBeNull] string text = null)
      : base(new Text(text ?? ""))
    {
    }

    public Paragraph([ItemNotNull] params MessagePart[] items)
      : base(items)
    {
    }

    public static implicit operator Paragraph([NotNull] string value)
    {
      return new Paragraph(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.Append("<div>");

      base.ToHtml(builder);

      builder.AppendLine("</div>");
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      builder.AppendLine();
      builder.Append(shift);

      base.ToPlainText(builder, shift);
    }
  }
}