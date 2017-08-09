namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using System.Collections.Generic;
  using JetBrains.Annotations;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  public interface ISitecoreDefaultsContext : ISitecoreConfigurationContext, ISitecoreAssembliesContext
  {
    /// <summary>
    ///   The default Sitecore Databases of the given Sitecore version.
    /// </summary>
    /// <exception cref="ResourceNotAvailableException">The exception will be thrown if resource is not available.</exception>
    [NotNull]
    IReadOnlyDictionary<string, IReleaseDefaultDatabase> Databases { get; }
  }
}