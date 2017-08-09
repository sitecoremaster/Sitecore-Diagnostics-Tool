namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using JetBrains.Annotations;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class EventQueueSize : AbstractQueueSizeTest
  {
    protected override string TableName { get; } = "EventQueue";

    protected override int Recommended => 5000;
  }
}