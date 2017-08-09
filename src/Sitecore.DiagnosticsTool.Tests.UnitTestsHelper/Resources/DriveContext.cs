namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System.Collections.Generic;
  using System.Linq;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.FileSystem;

  public class DriveContext : IDriveContext
  {
    private IEnumerable<Drive> Drives { get; }

    public DriveContext([CanBeNull] params Drive[] drives)
      : this((IEnumerable<Drive>)drives)
    {
    }

    public DriveContext(IEnumerable<Drive> drives)
    {
      Drives = drives ?? new Drive[0];
    }

    public Size GetAvailableFreeSpace(string driveName)
    {
      Assert.ArgumentNotNull(driveName, nameof(driveName));

      return Drives.First(x => x.Name == driveName).AvailableFreeSpace;
    }
  }
}