namespace Sitecore.DiagnosticsTool.Service.Controllers
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.IO.Compression;
  using System.Net;
  using System.Reflection;
  using System.Web;
  using System.Web.Mvc;
  using Microsoft.ApplicationInsights;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.Reporting;
  using Sitecore.DiagnosticsTool.Service.ErrorHandler;
  using Sitecore.DiagnosticsTool.TestRunner;

  [Route("api/v1")]
  [AiHandleError]
  public class V1Controller : Controller
  {
    [HttpGet]
    public string Get(bool? ftp = null)
    {
      return "Send Aggregated SSPG package in POST request. " + (ftp == null ? "There is also optional ?ftp=true switch which will include report into given package and upload it to Sitecore FTP." : "");
    }

    [HttpPost]
    public string Post(bool ftp = false)
    {
      var file = HttpContext.Request.Files[0];
      Assert.IsNotNull(file);

      var fileSystem = new FileSystem();

      using (var inputTemp = fileSystem.CreateUniqueTempFolder())
      {
        var mega = inputTemp.GetChildFile(file.FileName);
        using (var output = mega.Open(FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
        {
          using (var input = file.InputStream)
          {
            input.CopyTo(output);
          }
        }

        try
        {
          var report = AnalyzeMegaPackage(mega, file);

          if (!ftp)
          {
            return report;
          }

          IncludeReportToMega(mega, report);

          UploadToFtp(mega);

          return $"Uploaded to FTP as {file.FileName}; {report}";
        }
        catch (Exception ex)
        {
          if (ftp)
          {
            UploadToFtp(mega);

            return $"Uploaded to FTP as {file.FileName}";
          }
          
          new TelemetryClient().TrackException(ex, new Dictionary<string, string>() { { "fileName", file.FileName }, { "fileSize", file.ContentLength.ToString() } });

          Response.StatusCode = 500;
          Response.StatusDescription = "InternalServerError";
          
          return ex.PrintException();
        }
      }
    }

    private static string AnalyzeMegaPackage(IFile mega, HttpPostedFileBase file)
    {
      var packages = PackageHelper.ExtractMegaPackage(mega)
        .ToArray(x =>
          new SupportPackageDataProvider(x, null, null));

      try
      {
        Trace.TraceInformation($"Running tests, File = {file.FileName}");

        var assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
        var system = new SystemContext(assemblyName);
        var resultsFile = TestRunner.RunTests(packages, system, (test, index, count) => Trace.TraceInformation($"Running test #{index:D2}, File = {file.FileName}, Test = {test?.Name}"));
        var report = ReportBuilder.GenerateReport(resultsFile);

        return report;
      }
      finally
      {
        foreach (var package in packages)
        {
          package?.Dispose();
        }
      }
    }

    private void IncludeReportToMega(IFile mega, string report)
    {
      using (var zip = ZipFile.Open(mega.FullName, ZipArchiveMode.Update))
      {
        var entry = zip.GetEntry("index.html");
        if (entry != null)
        {
          return;
        }

        entry = zip.CreateEntry("index.html");
        using (var writer = new StreamWriter(entry.Open()))
        {
          writer.Write(report);
        }
      }
    }

    private void UploadToFtp(IFile packageFile)
    {
      Trace.TraceInformation($"Uploadting to FTP, File = {packageFile.Name}");

      var url = $"ftp://dl.sitecore.net/upload/{packageFile.Name}";
      var ftpRquest = (FtpWebRequest)WebRequest.Create(new Uri(url));
      ftpRquest.Credentials = new NetworkCredential();
      ftpRquest.KeepAlive = false;
      ftpRquest.UseBinary = true;
      ftpRquest.ContentLength = packageFile.Length;
      ftpRquest.Method = WebRequestMethods.Ftp.UploadFile;
      ftpRquest.EnableSsl = true;

      using (var sourceStream = packageFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        using (var targetStream = ftpRquest.GetRequestStream())
        {
          sourceStream.CopyTo(targetStream);
        }
      }
    }
  }
}