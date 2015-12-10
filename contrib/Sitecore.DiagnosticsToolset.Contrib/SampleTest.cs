namespace Sitecore.DiagnosticsToolset.Contrib
{
  using System.Collections.Generic;
  using System.Linq;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Objects;
  using Sitecore.DiagnosticsToolset.Core.Categories;
  using Sitecore.DiagnosticsToolset.Core.Extensions;
  using Sitecore.DiagnosticsToolset.Core.Resources;
  using Sitecore.DiagnosticsToolset.Core.Resources.Database;
  using Sitecore.DiagnosticsToolset.Core.Resources.Logging;
  using Sitecore.DiagnosticsToolset.Core.Resources.WebServer;
  using Sitecore.DiagnosticsToolset.Core.Tests;

  public class SampleTest : Test
  {
    /// <summary>
    /// Easy to remember and share test name.
    /// </summary>
    public override string Name
    {
      get
      {
        return "Sample test";
      }
    }

    /// <summary>
    /// The list of categories the test belongs to.
    /// </summary>
    public override IEnumerable<Category> Categories
    {
      get
      {
        yield return Category.General;
      }
    }

    /// <summary>
    /// The method indicates if this specific test is actual for Sitecore version of the instance under test.
    /// </summary>
    public override bool IsActual(ICollection<Category> categories, SitecoreVersion sitecoreVersion)
    {
      return base.IsActual(categories, sitecoreVersion) && sitecoreVersion.MajorInt >= 8;
    }

    /// <summary>
    /// All the test logic must be placed here. Use context parameter to access the API.
    /// </summary>
    /// <param name="context">A single interface to access API.</param>
    public override void Process(ITestContext context)
    {
      // list of all categories selected by user via UI
      var categories = context.Categories;
      if (categories.Contains(Category.Production))
      {
        // context.Passed is optional and should be used only if test is passed with special message
        context.Passed("Nothing to check if it is production environment");

        return;
      }

      CheckConfiguration(context);

      CheckWebDatabase(context);

      CheckLogs(context);

      CheckIIS(context);
    }

    private void CheckIIS(ITestContext context)
    {
      var currentSite = context.WebServer.CurrentSite;
      var applicationPool = currentSite.ApplicationPool;
      var managedPipelineMode = applicationPool.ManagedPipelineMode;
      if (managedPipelineMode == PipelineMode.Classic)
      {
        context.Warning("The application pool is configured to be run with classic mode which can be a administration mistake");
      }
    }

    private static void CheckLogs(ITestContext context)
    {
      // only last 200MB of logs can be parsed
      var logs = context.Logs.GetSitecoreLogEntries(LogLevel.Error);

      foreach (var error in logs)
      {
        context.Warning("Potential error in the log file: \r\n{0}", error.Message);
      }
    }

    private static void CheckWebDatabase(ITestContext context)
    {
      var web = context.Databases.Sql["web"];
      if (web == null)
      {
        return;
      }

      // create read item context to instantiate caches - they will be destroyed when it is disposed
      using (var items = web.Items.CreateContext())
      {
        // LinqToSql count how many home items in web db (https://github.com/Sitecore/Sitecore.Diagnostics.SqlDataProvider)
        var homeItemsCount = items.GetItems().Count(x => x.Name == "Home");
        if (homeItemsCount == 0)
        {
          context.Error("The /sitecore/content/Home item is missing");
        }

        // lets validate workflows (custom lightweight data provider Sitecore.DiagnosticsToolset.Resources.Database.dll)
        var workflowsRoot = items.GetItem(ItemIDs.WorkflowRoot, new GetItemOptions { LoadChildren = true, LoadDescendants = false });
        if (workflowsRoot == null)
        {
          context.Error("The /sitecore/system/workflows item is missing");

          return;
        }

        foreach (var workflow in workflowsRoot.Children)
        {
          var initialStepId = workflow.Fields.Shared["Initial"];
          if (string.IsNullOrEmpty(initialStepId))
          {
            context.Error("The {0} workflow does not have initial workflow step set up", workflow.ItemPath);
          }
        }
      }
    }

    private static void CheckConfiguration(ITestContext context)
    {
      // SitecoreInfo contains configuration, version and defaults
      var sitecoreInfo = context.SitecoreInfo;

      // showconfig.xml combined with web.config
      var configuration = sitecoreInfo.Configuration;
      if (configuration.SelectSingleElement("/configuration/sitecore") == null)
      {
        context.Error("The configuration is broken - missing <sitecore>...</sitecore> element");

        return;
      }

      var pipeline = sitecoreInfo.GetPipeline("httpRequestBegin");
      if (pipeline == null)
      {
        context.Error("The httpRequestBegin pipeline is not registered");

        return;
      }

      // check specific processor in httpRequestBegin pipeline
      var executeRequestType = TypeRef.Parse("Sitecore.Pipelines.HttpRequest.ExecuteRequest, Sitecore.Kernel");
      var executeRequest = pipeline.Processors.FirstOrDefault(x => x.Type == executeRequestType);
      if (executeRequest == null)
      {
        context.Warning("The {0} processor is missing which potentially can be configuration error", executeRequestType);
      }

      // download defaults for the given Sitecore version from cloud 
      var defaults = sitecoreInfo.SitecoreDefaults;

      // check all default processors
      var defaultPipeline = defaults.GetPipeline("httpRequestBegin");
      Assert.IsNotNull(defaultPipeline, "The default httpRequestBegin pipeline cannot be null");

      foreach (var defaultProcessor in defaultPipeline.Processors)
      {
        Assert.IsNotNull(defaultProcessor, "The default processor in httpRequestBegin pipeline cannot be null");

        if (defaultProcessor.Type == executeRequestType)
        {
          // we already checked that manually
          continue;
        }

        var actualProcessor = pipeline.Processors.FirstOrDefault(x => x.Type == defaultProcessor.Type);
        if (actualProcessor == null)
        {
          context.Warning("The {0} processor is missing which potentially can be configuration error", defaultProcessor.Type);
        }
      }
    }
  }
}
