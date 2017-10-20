namespace Sitecore.DiagnosticsTool.Tests.UnitTestsHelper.Resources
{
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;

  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.InfoService.Client;
  using Sitecore.Diagnostics.Objects;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.DataProviders;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Configuration;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.SitecoreInformation;

  public class SitecoreInstance : IDataProvider
  {
    private static readonly IServiceClient DefaultServiceClient = new ServiceClient();

    public IServiceClient ServiceClient { get; set; }

    public ISitecoreVersion Version { get; set; }

    public AssemblyFile[] Assemblies { get; set; }

    public XmlDocument Configuration { get; set; }

    public string GlobalAsaxFile { get; set; }

    public IEnumerable<PipelineDefinition> Pipelines { get; set; }

    public IEnumerable<PipelineDefinition> DefaultPipelines { get; set; }

    public string InstanceName { get; set; }

    public ServerRole[] ServerRoles { get; set; }

    public Map<IReleaseInfo> InstalledModules { get; set; }

    public IEnumerable<IResource> GetResources()
    {
      yield return new ServerRolesContext(ServerRoles.EmptyToNull() ?? new[] { ServerRole.ContentManagement | ServerRole.ContentDelivery | ServerRole.Publishing | ServerRole.ContentIndexing });

      var context = new GenericSitecoreInformationContext(ServiceClient ?? DefaultServiceClient);
      var instanceName = InstanceName;
      if (!string.IsNullOrEmpty(instanceName))
      {
        context.InstanceName = instanceName;
      }

      var assemblies = Assemblies;
      if (assemblies != null)
      {
        context.Assemblies = new AssemblyFileCollection(assemblies);
      }

      var version = Version;
      if (version != null)
      {
        context.SetVersion(version);
      }

      var configuration = Configuration;
      if (configuration != null)
      {
        context.Configuration = configuration;
        context.WebConfigFile = configuration;
      }

      var globalAsax = GlobalAsaxFile;
      if (globalAsax != null)
      {
        context.GlobalAsaxFile = globalAsax;
      }

      var pipelines = Pipelines;
      if (pipelines != null)
      {
        context.Pipelines = pipelines.ToDictionary(x => x.Name, x => x);
      }

      var defaultPipelines = DefaultPipelines;
      if (defaultPipelines != null)
      {
        context.DefaultsContext.Pipelines = defaultPipelines.ToDictionary(x => x.Name, x => x);
      }

      yield return context;
    }
  }
}