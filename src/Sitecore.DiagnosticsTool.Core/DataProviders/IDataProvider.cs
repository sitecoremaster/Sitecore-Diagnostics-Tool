namespace Sitecore.DiagnosticsTool.Core.DataProviders
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;

  /// <summary>
  ///   The data provider represents a way of retrieving data like LocalServer or SSPG package.
  /// </summary>
  public interface IDataProvider : IInstanceName
  {
    /// <summary>
    ///   The method returns the resources that data provider can offer.
    /// </summary>
    [ItemNotNull]
    [NotNull]
    IEnumerable<IResource> GetResources();
  }
}