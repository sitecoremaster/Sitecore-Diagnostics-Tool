namespace Sitecore.DiagnosticsTool.Core.Resources.FileSystem
{
  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;

  public interface IDriveContext
  {
    Size GetAvailableFreeSpace([NotNull] string driveName);
  }
}