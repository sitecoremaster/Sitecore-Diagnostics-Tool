namespace Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation
{
  using JetBrains.Annotations;

  public interface IInstanceName
  {
    /// <summary>
    ///   The name of the Sitecore instance.
    /// </summary>
    [NotNull]
    string InstanceName { get; }
  }
}