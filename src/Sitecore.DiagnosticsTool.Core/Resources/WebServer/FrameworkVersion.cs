namespace Sitecore.DiagnosticsTool.Core.Resources.WebServer
{
  /// <summary>
  ///   Represents the version of .NET Framework. Example:
  ///   <example>
  ///     if (FrameworkVersion.45x.HasFlag(framework))
  ///     {
  ///     /* framework is 450 or 451 or 452 */
  ///     }
  ///   </example>
  /// </summary>
  // ReSharper disable InconsistentNaming
  public enum FrameworkVersion
  {
    /// <summary>
    ///   .NET Framework 2.0 specifically
    /// </summary>
    v20 = 1,

    /// <summary>
    ///   .NET Framework 3.0 specifically
    /// </summary>
    v30 = 2,

    /// <summary>
    ///   .NET Framework 3.5 specifically
    /// </summary>
    v35 = 4,

    /// <summary>
    ///   .NET Framework 4.0 specifically
    /// </summary>
    v40 = 8,

    /// <summary>
    ///   .NET Framework 4.5.0 specifically
    /// </summary>
    v450 = 16,

    /// <summary>
    ///   .NET Framework 4.5.1 or further
    /// </summary>
    v451 = 32,

    /// <summary>
    ///   .NET Framework 4.5.1 or further
    /// </summary>
    v452 = 64,

    /// <summary>
    ///   .NET Framework 4.5.1 or further
    /// </summary>
    v460 = 128,

    /// <summary>
    ///   .NET Framework 4.5.x which includes 4.5.0, 4.5.1, 4.5.2, 4.6.0
    /// </summary>
    v45x = v450 | v451 | v452 | v460,

    /// <summary>
    ///   .NET Framework 4.x which includes 4.0, 4.5.0, 4.5.1, 4.5.2, 4.6.0
    /// </summary>
    v4x = v40 | v45x,

    /// <summary>
    ///   .NET Framework 2.x which includes 2.0, 3.0, 3.5
    /// </summary>
    v2x = v20 | v30 | v35,

    /// <summary>
    ///   Unknown version of .NET Framework
    /// </summary>
    Undefined = 666666
  }

  // ReSharper restore InconsistentNaming
}