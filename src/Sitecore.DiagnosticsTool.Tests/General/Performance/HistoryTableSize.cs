namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using JetBrains.Annotations;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class HistoryTableSize : AbstractQueueSizeTest
  {
    protected override string TableName { get; } = "History";

    protected override int Recommended => 50000;
  }
}