namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Text;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;

  public class Container : MessagePart
  {
    [NotNull]
    [ItemNotNull]
    public MessagePart[] Items { get; }

    public Container([ItemNotNull] params MessagePart[] items)
    {
      Assert.ItemsNotNull(items);

      Items = items;
    }

    public override void ToHtml(StringBuilder builder)
    {
      foreach (var innerPart in Items)
      {
        ToHtml(builder, innerPart);
      }
    }

    protected virtual void ToHtml([NotNull] StringBuilder builder, [NotNull] MessagePart innerPart)
    {
      innerPart.ToHtml(builder);
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      foreach (var innerPart in Items)
      {
        ToPlainText(builder, innerPart, shift);
      }
    }

    protected virtual void ToPlainText([NotNull] StringBuilder builder, [NotNull] MessagePart innerPart, [CanBeNull] string shift = null)
    {
      innerPart.ToPlainText(builder, shift);
    }
  }
}