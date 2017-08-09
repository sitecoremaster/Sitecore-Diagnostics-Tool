namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Helpers.XPath
{
  using System;
  using JetBrains.Annotations;

  public class InvalidXPathException : InvalidOperationException
  {
    public int Position { get; }

    [NotNull]
    public string XPath { get; }

    public InvalidXPathException(string description, string xpath, int position)
      : base("Invalid xpath (" + description + ")")
    {
      XPath = xpath;
      Position = position;
    }
  }
}