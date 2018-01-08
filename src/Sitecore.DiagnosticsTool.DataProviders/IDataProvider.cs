namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage
{
  using System.Collections.Generic;

  using JetBrains.Annotations;

  using Sitecore.DiagnosticsTool.Core.Resources.Common;

  /// <summary>
  ///   The data provider represents a way of retrieving data like LocalServer or SSPG package.
  /// </summary>
  public interface IDataProvider
  {
    /// <summary>
    ///   The method returns the resources that data provider can offer.
    /// </summary>
    [ItemNotNull]
    [NotNull]
    IEnumerable<IResource> GetResources();
  }
}
