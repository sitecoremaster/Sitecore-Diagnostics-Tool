namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  /// <summary>
  ///   The base context interface for providing access to solution-wide resources.
  /// </summary>
  public interface ISolutionTestResourceContext : IReadOnlyDictionary<string, ITestResourceContext>, IInstanceName, ITestResourceContextBase
  {
  }
}