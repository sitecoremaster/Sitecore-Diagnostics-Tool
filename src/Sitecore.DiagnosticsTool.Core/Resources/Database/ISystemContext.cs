namespace Sitecore.DiagnosticsTool.Core.Resources.Database
{
  using System;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   System information.
  /// </summary>
  public interface ISystemContext : IResource
  {
    /// <summary>
    ///   Reserved for system use.
    /// </summary>
    [Obsolete("Reserved for system use")]
    string DiagCode { get; }

    /// <summary>
    ///   Reserved for system use.
    /// </summary>
    [Obsolete("Reserved for system use")]
    string ApplicationInfo { get; }
  }
}