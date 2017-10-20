namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System;
  using System.Text;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;

  public class Link : Text
  {
    [NotNull]
    public string AbsoluteUri { get; }

    public Link([NotNull] Uri link, [CanBeNull] string text = null)
      : base(text.EmptyToNull() ?? link.AbsoluteUri)
    {
      AbsoluteUri = link.AbsoluteUri;
    }

    public static implicit operator Link([NotNull] Uri value)
    {
      return new Link(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.Append($"<a href='{AbsoluteUri}'>");

      base.ToHtml(builder);

      builder.Append($"</a>");
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      if (string.IsNullOrWhiteSpace(Value) || Value.Equals(AbsoluteUri, StringComparison.OrdinalIgnoreCase))
      {
        base.ToPlainText(builder, shift);

        return;
      }

      builder.Append($"[{Value}]({AbsoluteUri})");
    }
  }
}