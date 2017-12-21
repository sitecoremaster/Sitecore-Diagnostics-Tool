// <summary>
//   The read-only interface for accessing File System
// </summary>

namespace Sitecore.DiagnosticsTool.Core.Resources.FileSystem
{
  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   The read-only interface for accessing File System
  /// </summary>
  public interface IFileSystemContext : IResource
  {
    /// <summary>
    ///   The read-only interface for accessing drive settings
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IDriveContext Drives { get; }
  }
}