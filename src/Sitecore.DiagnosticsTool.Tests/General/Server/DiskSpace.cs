namespace Sitecore.DiagnosticsTool.Tests.General.Server
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Tests;

  // Reviewed: OK (2017-06-13, looks valid)
  [UsedImplicitly]
  public class DiskSpace : Test
  {
    protected const int Minimum = 1;

    protected const int Recommended = 10;

    public override string Name { get; } = "Sufficient free space on the disk";

    public override IEnumerable<Category> Categories { get; } = new[] {Category.General};

    public override void Process(ITestResourceContext data, ITestOutputContext output)
    {
      Assert.ArgumentNotNull(data, nameof(data));

      var driveName = data.WebServer.CurrentSite.WebRoot.Root.Name;
      var size = data.FileSystem.Drives.GetAvailableFreeSpace(driveName);
      if (size.GB < Minimum)
      {
        output.Error(GetErrorMessage(size, driveName));
      }
      else if (size.GB < Recommended)
      {
        output.Warning(GetWarningMessage(size, driveName));
      }
    }

    [NotNull]
    protected string GetErrorMessage(Size size, [NotNull] string driveName)
    {
      Assert.ArgumentNotNull(driveName, nameof(driveName));

      return $"Web Server has {size} GB of available free disk space on {driveName} drive, which is less than {Recommended} minimum recommended by Sitecore";
    }

    [NotNull]
    protected string GetWarningMessage(Size size, [NotNull] string driveName)
    {
      Assert.ArgumentNotNull(driveName, nameof(driveName));

      return $"Web Server has {size.GB} GB of available free disk space on {driveName} drive, it is recommended to monitor its value to prevent going less than reasonable minimum.";
    }
  }
}