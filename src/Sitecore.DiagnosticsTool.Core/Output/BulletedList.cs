namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;

  public class BulletedList : Container
  {
    public BulletedList([NotNull] params MessagePart[] items)
      : base(items)
    {
      Assert.ItemsNotNull(items);
    }

    public BulletedList([NotNull] params object[] items)
      : this((IEnumerable<object>)items)
    {
      Assert.ItemsNotNull(items);
    }

    public BulletedList([NotNull] IEnumerable<object> arr)
      : base(arr.Select(x => x.ToString()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray(x => new Text(x)))
    {
    }

    public static implicit operator BulletedList([NotNull] string[] value)
    {
      return new BulletedList(value);
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.AppendLine($"<ul>");

      base.ToHtml(builder);

      builder.AppendLine($"</ul>");
    }

    protected override void ToHtml(StringBuilder builder, MessagePart innerPart)
    {
      builder.Append("<li>");

      base.ToHtml(builder, innerPart);

      builder.AppendLine("</li>");
    }

    protected override void ToPlainText(StringBuilder builder, MessagePart innerPart, string shift = null)
    {
      builder.AppendLine();
      builder.Append(shift);

      builder.Append("* ");

      base.ToPlainText(builder, innerPart, shift + "  ");
    }

    public static BulletedList Create<T>([NotNull] IEnumerable<T> arr, [NotNull] Func<T, string> func)
    {
      return new BulletedList(arr.ToArray(func));
    }
  }
}