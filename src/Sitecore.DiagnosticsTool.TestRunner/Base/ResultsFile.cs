namespace Sitecore.DiagnosticsTool.TestRunner.Base
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;

  public sealed class ResultsFile
  {
    [CanBeNull]
    public Map<IDataProvider> Packages { [UsedImplicitly] get; set; }

    public ITestReport[] Solution { [UsedImplicitly] get; set; }
  }
}