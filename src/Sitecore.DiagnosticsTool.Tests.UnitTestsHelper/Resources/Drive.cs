namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;

  public class Drive
  {
    public Drive([NotNull] string driveName)
    {
      Assert.ArgumentNotNull(driveName, nameof(driveName));
      Name = driveName;
    }

    public Size AvailableFreeSpace { get; set; }

    public string Name { get; }
  }
}