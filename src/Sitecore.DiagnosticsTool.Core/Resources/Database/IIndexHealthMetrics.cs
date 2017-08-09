namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  public interface IIndexHealthMetrics
  {
    string Name { get; }

    double AverageFragmentationInPercent { get; }

    long PageCount { get; }
  }
}