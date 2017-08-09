namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;

  public class Table : MessagePart
  {
    [NotNull]
    [ItemNotNull]
    public TableRow[] Rows { get; }

    [NotNull]
    [ItemNotNull]
    public string[] Headers { get; }
    
    public Table([ItemNotNull] params TableRow[] rows)
      : this(new string[0], rows)
    {
    }

    public Table([NotNull] IEnumerable<string> headers, [ItemNotNull] params TableRow[] rows)
    {
      Assert.ItemsNotNull(rows);

      Headers = new string[0]
        .Concat(headers)
        .Concat(rows.SelectMany(x => x.Headers))
        .Where(Str.IsNotNullOrWhiteSpace)
        .Distinct()
        .ToArray();

      Rows = rows;
    }

    public override void ToHtml(StringBuilder builder)
    {
      builder.AppendLine("<table class='table table-striped'>");

      // header
      builder.AppendLine("<tr>");

      foreach (var header in Headers)
      {
        builder.AppendLine("<th>");
        builder.AppendLine(header);
        builder.AppendLine("</th>");
      }

      builder.AppendLine("</tr>");

      // rows
      foreach (var row in Rows)
      {
        ToHtml(builder, row);
      }

      builder.AppendLine("</table>");
    }

    protected virtual void ToHtml([NotNull] StringBuilder builder, [NotNull] TableRow row)
    {
      builder.AppendLine("<tr>");

      var cols = row
        .Select(x => new
        {
          Header = x.Key,
          SortOrder = Array.IndexOf(Headers, x.Key),
          Value = x.Value
        }).ToDictionary(x => x.SortOrder, x => x.Value);

      for(var i=0;i<Headers.Length;++i)
      {
        var col = cols.TryGetValue(i) ?? "";
        builder.AppendLine("<td>");
        builder.AppendLine(col);
        builder.AppendLine("</td>");
      }

      builder.AppendLine("</tr>");
    }

    public override void ToPlainText(StringBuilder builder, string shift = null)
    {
      var index = 1;
      foreach (var row in Rows)
      {
        ToPlainText(builder, row, index++, shift);
      }
    }

    protected virtual void ToPlainText([NotNull] StringBuilder builder, [NotNull] TableRow row, int index, [CanBeNull] string shift = null)
    {
      var cols = row
        .Select(x => new
        {
          Header = x.Key,
          SortOrder = Array.IndexOf(Headers, x.Key),
          Value = x.Value
        })
        .OrderBy(x => x.SortOrder != -1 ? x.SortOrder : int.MaxValue)
        .ToArray();
        
      var col0 = cols.First();
      if (col0.SortOrder == 0)
      {
        builder.AppendLine();
        builder.Append(shift);

        builder.Append("* ");
        builder.Append(col0.Value);

        cols = cols.Skip(1).ToArray();
      }
      else
      {
        builder.AppendLine();
        builder.Append(shift);

        builder.Append("* Row ");
        builder.Append(index);
      }

      foreach (var col in cols)
      {
        builder.AppendLine();
        builder.Append(shift + "  ");

        builder.Append("* ");
        builder.Append(col.Header);
        builder.Append(" = ");
        builder.Append(col.Value);
      }
    }
  }
}