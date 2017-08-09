namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper
{
  public static class StringExtensions
  {
    public static bool ContainsIgnoreCase(this string that, string contains)
    {
      return that.ToLower().Contains(contains.ToLower());
    }
  }
}