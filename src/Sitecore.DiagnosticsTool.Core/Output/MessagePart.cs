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

    public override bool Equals(object obj)
    {
      var other = obj as MessagePart;

      return other != null ? Equals(other) : base.Equals(obj);
    }

    public bool Equals([CanBeNull] MessagePart obj)
    {
      return Equals(this, obj);
    }

    public static bool Equals([CanBeNull] MessagePart left, [CanBeNull] MessagePart right)
    {
      return string.Equals(left?.ToString(), right?.ToString());
    }

    public override int GetHashCode()
    {
      return ToString().GetHashCode();
    }

    /// <summary>
    ///   Renders text in markdown format. Use overload to see other options.
    /// </summary>
    public sealed override string ToString()
    {
      return ToString(OutputFormat.Text);
    }

    public static bool operator ==(MessagePart left, MessagePart right)
    {
      return Equals(left, right);
    }

    public static bool operator !=(MessagePart left, MessagePart right)
    {
      return !(left == right);
    }

    public static implicit operator MessagePart(string text)
    {
      return new Text(text);
    }
  }
}