namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  public sealed class ShortMessage : Message
  {
    public ShortMessage([NotNull] IEnumerable<MessagePart> parts)
      : base(parts.ToArray())
    {
    }

    public ShortMessage([NotNull] params MessagePart[] parts)
      : base(parts)
    {
    }

    public static implicit operator ShortMessage(string value)
    {
      return new ShortMessage(new Text(value));
    }
  }
}