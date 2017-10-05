namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation
{
  using System.Collections;
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Collections;

  public class AssemblyFileCollection : IReadOnlyDictionary<string, AssemblyFile>
  {
    private Map<AssemblyFile> Data { get; }

    public AssemblyFileCollection(IEnumerable<AssemblyFile> data)
    {
      Data = data.ToMap(x => x.FileName.ToLower(), x => x);
    }

    public IEnumerator<KeyValuePair<string, AssemblyFile>> GetEnumerator()
    {
      return Data.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public int Count => Data.Count;

    public bool ContainsKey(string key)
    {
      return Data.ContainsKey(key.ToLowerInvariant());
    }

    public bool TryGetValue(string key, out AssemblyFile value)
    {
      return Data.TryGetValue(key.ToLowerInvariant(), out value);
    }

    [NotNull]
    public AssemblyFile this[string key] => Data[key.ToLowerInvariant()];

    [ItemNotNull]
    public IEnumerable<string> Keys => Data.Keys;

    [ItemNotNull]
    public IEnumerable<AssemblyFile> Values => Data.Values;
  }
}