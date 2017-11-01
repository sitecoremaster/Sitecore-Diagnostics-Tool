namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Database
{
  using System;

  [Serializable]
  public class InvalidQueryException : Exception
  {
    public InvalidQueryException()
      : base("Query is invalid or contains illegal SQL statement.")
    {
    }
  }
}