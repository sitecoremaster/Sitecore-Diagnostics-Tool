namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  public class GenericSitecoreInformationContext : SitecoreInformationContext
  {
    [CanBeNull]
    private SitecoreDefaultsContext _DefaultsContext;

    [NotNull]
    public SitecoreDefaultsContext DefaultsContext => _DefaultsContext ?? (_DefaultsContext = new SitecoreDefaultsContext(SitecoreVersion, _ServiceClient));

    public override ISitecoreDefaultsContext SitecoreDefaults => DefaultsContext;

    public override ISitecoreVersion SitecoreVersion => Version ?? base.SitecoreVersion;

    // for unit testing

    public ISitecoreVersion Version { get; set; }

    public void SetVersion([NotNull] ISitecoreVersion version)
    {
      Assert.ArgumentNotNull(version, nameof(version));

      Version = version;
    }

    public GenericSitecoreInformationContext(IServiceClient client) : base(client)
    {
    }
  }
}