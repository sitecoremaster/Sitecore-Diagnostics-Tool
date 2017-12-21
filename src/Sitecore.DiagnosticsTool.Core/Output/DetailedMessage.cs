namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  public sealed class DetailedMessage : Message
  {
    public DetailedMessage([NotNull] IEnumerable<MessagePart> parts)
      : base(parts.ToArray())
    {
    }

    public DetailedMessage([NotNull] params MessagePart[] parts)
      : base(parts)
    {
    }

    public static implicit operator DetailedMessage(string value)
    {
      return new DetailedMessage(new Text(value));
    }
  }
}