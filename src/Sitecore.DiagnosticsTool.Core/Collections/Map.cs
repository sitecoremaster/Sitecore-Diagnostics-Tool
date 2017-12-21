namespace Sitecore.DiagnosticsTool.Core.Collections
{
  using System;
  using System.Collections.Generic;
  using System.Runtime.Serialization;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public class Map : Dictionary<string, string>
  {
    [CanBeNull]
    private Func<string, string> KeyInitializer { get; }

    public Map()
    {
    }

    public Map(Func<string, string> keyInitializer)
    {
      KeyInitializer = keyInitializer;
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

    public void Add(string value)
    {
      Assert.IsNotNull(KeyInitializer);

      Add(KeyInitializer(value), value);
    }
  }

  public class Map<T> : Dictionary<string, T>
  {
    [CanBeNull]
    private Func<T, string> KeyInitializer { get; }

    public Map()
    {
    }

    public Map(Func<T, string> keyInitializer)
    {
      KeyInitializer = keyInitializer;
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

    public void Add(T value)
    {
      Assert.IsNotNull(KeyInitializer);

      Add(KeyInitializer(value), value);
    }
  }
}