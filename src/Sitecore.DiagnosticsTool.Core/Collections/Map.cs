namespace Sitecore.DiagnosticsTool.Core.Collections
{
  using System.Collections.Generic;
  using System.Runtime.Serialization;

  using JetBrains.Annotations;

  public class Map : Dictionary<string, string>
  {
    public Map()
    {
    }

    public Map(int capacity)
      : base(capacity)
    {
    }

    public Map(IEqualityComparer<string> comparer)
      : base(comparer)
    {
    }

    public Map(int capacity, IEqualityComparer<string> comparer)
      : base(capacity, comparer)
    {
    }

    public Map([NotNull] IDictionary<string, string> dictionary)
      : base(dictionary)
    {
    }

    public Map([NotNull] IDictionary<string, string> dictionary, IEqualityComparer<string> comparer)
      : base(dictionary, comparer)
    {
    }

    protected Map(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }

  public class Map<T> : Dictionary<string, T>
  {
    public Map()
    {
    }

    public Map(int capacity)
      : base(capacity)
    {
    }

    public Map(IEqualityComparer<string> comparer)
      : base(comparer)
    {
    }

    public Map(int capacity, IEqualityComparer<string> comparer)
      : base(capacity, comparer)
    {
    }

    public Map([NotNull] IDictionary<string, T> dictionary)
      : base(dictionary)
    {
    }

    public Map([NotNull] IDictionary<string, T> dictionary, IEqualityComparer<string> comparer)
      : base(dictionary, comparer)
    {
    }

    protected Map(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }
  }
}