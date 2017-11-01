namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Collections.Generic;
  using System.Linq;

  using JetBrains.Annotations;

  public abstract class Message : Container
  {
    public Message([NotNull] IEnumerable<MessagePart> parts)
      : base(parts.ToArray())
    {
    }
  }
}