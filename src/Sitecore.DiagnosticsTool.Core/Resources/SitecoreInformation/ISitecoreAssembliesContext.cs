namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  public interface ISitecoreAssembliesContext
  {
    /// <summary>
    ///   The Website\bin\*.dll files.
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IReadOnlyDictionary<string, AssemblyFile> Assemblies { get; }
  }
}