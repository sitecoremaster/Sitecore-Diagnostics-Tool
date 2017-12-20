namespace Sitecore.DiagnosticsTool.Core.Tests
{
  using System.Collections.Generic;

  /// <summary>
  ///   The base context interface for providing access to solution-wide resources.
  /// </summary>
  public interface ISolutionTestResourceContext : IReadOnlyDictionary<string, ITestResourceContext>, ITestResourceContextBase
  {
  }
}