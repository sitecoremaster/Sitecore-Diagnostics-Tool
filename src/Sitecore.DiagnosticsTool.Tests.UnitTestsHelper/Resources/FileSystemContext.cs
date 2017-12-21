namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using Sitecore.DiagnosticsTool.Core.Resources.FileSystem;

  public class FileSystemContext : IFileSystemContext
  {
    public IDriveContext Drives { get; set; }
  }
}