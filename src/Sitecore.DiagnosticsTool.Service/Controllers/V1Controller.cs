namespace Sitecore.DiagnosticsTool.Service.Controllers
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Reflection;
  using System.Web;
  using System.Web.Mvc;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.Reporting;

  [Route("api/v1")]
  public class V1Controller : Controller
  {
    [HttpGet]
    public string Get()
    {
      return "Send Aggregated SSPG package in POST request.";
    }

    [HttpPost]
    public string Post()
    {

      var file = HttpContext.Request.Files[0];
      Assert.IsNotNull(file);

      var fileSystem = new FileSystem();

      using (var inputTemp = fileSystem.CreateUniqueTempFolder())
      {
        var mega = inputTemp.GetChildFile("1.zip");//file.FileName);
        using (var output = mega.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
          using (var input = file.InputStream)
          {
            input.CopyTo(output);
          }
        }

        var packages = PackageHelper.ExtractMegaPackage(mega)
              .ToArray(x =>
                new SupportPackageDataProvider(x, null, null));

        try
        {
          Console.WriteLine("Running tests...");

          var assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
          var system = new SystemContext(assemblyName);
          var resultsFile = TestRunner.TestRunner.RunTests(packages, system, (test, index, count) => Console.WriteLine($"Running {test?.Name}..."));

          Console.WriteLine("Building report...");

          return ReportBuilder.GenerateReport(resultsFile);
        }
        finally
        {
          foreach (var package in packages)
          {
            package?.Dispose();
          }
        }
      }
    }
  }
}