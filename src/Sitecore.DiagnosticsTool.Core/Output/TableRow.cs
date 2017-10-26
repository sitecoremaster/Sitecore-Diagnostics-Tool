namespace Sitecore.DiagnosticsTool.Core.Output
{
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.DiagnosticsTool.Core.Collections;

  public class TableRow : Dictionary<string, string>
  {
    public TableRow()
    {
    }

    public TableRow([NotNull] params Pair[] collection)
      : this(collection as IEnumerable<Pair>)
    {
    }

    public TableRow([NotNull] IEnumerable<Pair> collection) 
      : base(collection.ToDictionary(x => x.Key, x => x.Value))
    {
    }

    public IReadOnlyList<string> Headers => this.ToArray(x => x.Key);
  }
}