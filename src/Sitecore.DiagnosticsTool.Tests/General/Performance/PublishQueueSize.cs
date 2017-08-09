namespace Sitecore.DiagnosticsTool.Tests.General.Performance
{
  using JetBrains.Annotations;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class PublishQueueSize : AbstractQueueSizeTest
  {
    protected override string TableName { get; } = "PublishQueue";

    protected override int Recommended => 50000;
  }
}