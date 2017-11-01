namespace Sitecore.DiagnosticsTool.Core.Collections
{
  public class Pair
  {
    public Pair(string key, string value)
    {
      Key = key;
      Value = value;
    }

    public string Key { get; set; }

    public string Value { get; set; }
  }
}