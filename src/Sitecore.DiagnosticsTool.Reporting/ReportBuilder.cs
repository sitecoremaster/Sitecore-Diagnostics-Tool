namespace Sitecore.DiagnosticsTool.Reporting
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.DictionaryExtensions;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.Base.Extensions.StringExtensions;
  using Sitecore.DiagnosticsTool.Core;
  using Sitecore.DiagnosticsTool.Core.Base;
  using Sitecore.DiagnosticsTool.Core.Collections;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Output;
  using Sitecore.DiagnosticsTool.Core.Resources;
  using Sitecore.DiagnosticsTool.Core.Resources.SitecoreInformation;
  using Sitecore.DiagnosticsTool.TestRunner;
  using Sitecore.DiagnosticsTool.TestRunner.Base;

  public static class ReportBuilder
  {
    [NotNull]
    public static string GenerateReport(ResultsFile resultsFile)
    {
      var template = File.ReadAllText(Application.GetEmbeddedFile(typeof(ReportBuilder).Assembly, "Template.html"));

      var errorMessages = Safe(_ => GetErrorMessages(resultsFile));
      var errorText = errorMessages
        .JoinToString("\r\n").EmptyToNull() ?? "<p>No error messages</p>";

      var warningMessages = Safe(_ => GetWarningMessages(resultsFile));
      var warningText = warningMessages
        .JoinToString("\r\n").EmptyToNull() ?? "<p>No warning messages</p>";

      var debugMessages = Safe(_ => GetDebugMessages(resultsFile));
      var debugText = debugMessages
        .JoinToString("\r\n").EmptyToNull() ?? "<p>No debug messages</p>";

      var cannotRunMessages = Safe(_ => GetCannotRunMessages(resultsFile));
      var cannotRunText = cannotRunMessages
        .JoinToString("\r\n").EmptyToNull() ?? "<p>No cannot run messages</p>";

      return template
        .Replace("<span class=\"placeholder-messages-total-count\"></span>", Safe(_ => GetMessagesTotalCount(errorMessages.Length, warningMessages.Length, resultsFile)))
        .Replace("<span class=\"placeholder-error-messages-count\"></span>", $"{errorMessages.Length}")
        .Replace("<span class=\"placeholder-warning-messages-count\"></span>", $"{warningMessages.Length}")
        .Replace("<span class=\"placeholder-run-tests-count\"></span>", Safe(_ => GetRunTestsCount(resultsFile)))
        .Replace("<span class=\"placeholder-total-tests-count\"></span>", Safe(_ => GetTotalTestsCount()))
        .Replace("<span class=\"placeholder-instances-count\"></span>", Safe(_ => GetInstancesCount(resultsFile)))
        .Replace("<span class=\"placeholder-report-created\"></span>", Safe(_ => GetReportCreated()))
        .Replace("<div class=\"placeholder-instances-roles\"></div>", Safe(_ => GetInstancesRoles(resultsFile)))
        .Replace("<div class=\"placeholder-instances-modules\"></div>", Safe(_ => GetInstancesModules(resultsFile)))
        .Replace("<div class=\"placeholder-error-messages\"></div>", Safe(_ => errorText))
        .Replace("<div class=\"placeholder-warning-messages\"></div>", warningText)
        .Replace("<div class=\"placeholder-cannot-run-messages\"></div>", cannotRunText)
        .Replace("<div class=\"placeholder-debug-messages\"></div>", debugText);
    }

    private static T Safe<T>([NotNull] Func<Null, T> func)
    {
      try
      {
        return func(null);
      }
      catch (Exception ex)
      {
        // TODO: save to log file and save to report

        return default(T);
      }
    }

    private static string GetMessagesTotalCount(int errorMessages, int warningMessages, ResultsFile resultsFile)
    {
      var count = errorMessages + warningMessages + resultsFile.Instances.Values.Sum(x => x.Count(z => z.Results.DebugLogs.Any(c => c != null)));

      return $"{count}";
    }

    private static string GetRunTestsCount(ResultsFile resultsFile)
    {
      var runTestsCount = resultsFile.Instances.Values
        .SelectMany(x => x.Select(z => z.Owner.Name))
        .GroupBy(x => x)
        .Count();

      return $"{runTestsCount}";
    }

    private static string GetTotalTestsCount()
    {
      return new TestManager().GetTests().Count().ToString();
    }

    private static string GetInstancesCount(ResultsFile resultsFile)
    {
      var instancesCount = $"{resultsFile.Instances.Count}";

      return instancesCount;
    }

    private static string GetReportCreated()
    {
      var utcNow = DateTime.UtcNow;
      var reportCreated = $"Created on {utcNow:yyyy MMMM dd} at {utcNow:HH:mm} UTC";

      return reportCreated;
    }

    private static string GetInstancesRoles(ResultsFile resultsFile)
    {
      var contexts = resultsFile.Packages.Values
        .ToArray(x => new
        {
          SitecoreInformationContext = x.GetResources().OfType<ISitecoreInformationContext>().FirstOrDefault(),
          RolesContext = x.GetResources().OfType<IServerRolesContext>().FirstOrDefault(),
        });

      var data = contexts
        .ToArray(x => new
        {
          InstanceName = x.SitecoreInformationContext?.InstanceName,
          Roles = x.RolesContext?.Select(z => z.ToString().EmptyToNull())
        });

      var instancesRoles = data
        .Where(x => x.InstanceName != null && x.Roles != null)
        .Select(x => $"<tr><td>{x.InstanceName}<td>{string.Join(", ", x.Roles)}")
        .JoinToString("\r\n");

      return instancesRoles;
    }

    private static string GetInstancesModules(ResultsFile resultsFile)
    {
      var contexts = resultsFile.Packages.Values
        .ToArray(x => 
          x.GetResources().OfType<ISitecoreInformationContext>().FirstOrDefault());

      var map = contexts.SelectMany(x => x.ModulesInformation.InstalledModules.Values.Select(z => new
        
        {
          InstanceName = x.InstanceName,
          ProductName = $"{z.Release.ProductName}", 
          Version = $"{z.Release.Version}",
        })).GroupBy(x => x.ProductName);

      if (!map.Any())
      {
        return "";
      }

      var sb = new StringBuilder();
      new Table(map.ToArray(x => 
        new TableRow(
          new[] { new Pair("Module", x.Key) }
            .Concat(x.Select(z => new Pair(z.InstanceName, z.Version)))
            .ToArray())))
        .ToHtml(sb);

      return "<h3>Modules:</h3><div>" + sb + "</div>";
    }

    [NotNull]
    private static string[] GetErrorMessages(ResultsFile resultsFile)
    {
      return GetMessages("danger", "E", resultsFile, r => r.Results.Errors.Select(Render)).ToArray();
    }

    [NotNull]
    private static string[] GetWarningMessages(ResultsFile resultsFile)
    {
      return GetMessages("warning", "W", resultsFile, r => r.Results.Warnings.Select(Render)).ToArray();
    }

    [NotNull]
    private static string[] GetDebugMessages(ResultsFile resultsFile)
    {
      return GetMessages("info", "D", resultsFile, r => 
        r.Results.DebugLogs.Select(m => m.Items.Length == 1 && m.Items.Single() is Text
          ? Render(m.Items.Single(), null, null) 
          : Render(new Text("There is additional debugging information"), null, m))).ToArray();
    }

    [NotNull]
    private static string[] GetCannotRunMessages(ResultsFile resultsFile)
    {
      return GetMessages("info", "C", resultsFile, r => r.Results.CannotRun.Select(Render)).ToArray();
    }

    private static string Render(ITestResult testResult)
    {
      var shortMessage = testResult.Message;
      var link = testResult.Link;
      var detailed = testResult.Detailed;

      return Render(shortMessage, link, detailed);
    }

    private static string Render(MessagePart shortMessage, Uri link, MessagePart detailed)
    {
      var result = shortMessage.ToString(OutputFormat.Html);
      if (link == null && detailed == null)
      {
        return result;
      }

      result += Token;
      if (link != null)
      {
        result += $"Get more information in <a href='{link.AbsoluteUri}'>this document</a>.<hr />";
      }
      else
      {
        result += detailed.ToString(OutputFormat.Html);
      }

      return result;
    }

    [NotNull]
    [ItemNotNull]
    private static IEnumerable<string> GetMessages(string alertType, string prefix, ResultsFile resultsFile, Func<ITestReport, IEnumerable<string>> getMessages)
    {
      var counter = 1;
      var testToInstanceToMessages = new Map<Map<List<string>>>();
      foreach (var packageName in resultsFile.Instances.Keys)
      {
        var reports = resultsFile.Instances[packageName];
        foreach (var report in reports)
        {
          var test = report.Owner.Name;
          var instanceToMessages = testToInstanceToMessages.GetOrAdd(test, new Map<List<string>>());
          Assert.IsNotNull(instanceToMessages);

          var messages = instanceToMessages.GetOrAdd(packageName, new List<string>());
          Assert.IsNotNull(messages);

          messages.AddRange(getMessages(report));
        }
      }

      foreach (var test in testToInstanceToMessages.Keys)
      {
        var instanceToMessages = testToInstanceToMessages[test];

        var instanceToMessage = new Map();
        foreach (var packageName in instanceToMessages.Keys)
        {
          var messages = instanceToMessages[packageName];
          if (messages.Count <= 0)
          {
            continue;
          }

          var validFor = "";
          if (!string.IsNullOrEmpty(packageName))
          {
            var infoContext = resultsFile.Packages[packageName].GetResources().OfType<ISitecoreInformationContext>().FirstOrDefault();
            validFor = infoContext?.InstanceName;
          }

          var messageContent = "";
          foreach (var message in messages)
          {
            messageContent += $"{message}<br />";
          }

          instanceToMessage.Add(validFor, messageContent);
        }

        var content = instanceToMessage.GroupBy(x => x.Value)
          .Select(g => new
          {
            Message = g.Key,
            Instances = g.ToArray()
          })
          .Select(g => $"{GetValidForText(g.Instances)}\r\n{g.Message}")
          .JoinToString("\r\n");

        if (string.IsNullOrWhiteSpace(content))
        {
          continue;
        }

        var id = $"{prefix}{counter++}";
        var header = $"" +
          $"<h4 class='alert-heading'>" +
          $"  <a href='#{id}'>{id}</a>. <span class='test-name'>{test}</span>" +
          $"</h4>";

        yield return GetMessage(alertType, header + Render(id, test, content));
      }

      foreach (ITestReport testReport in resultsFile.Solution)
      {
        var test = testReport.Owner.Name;
        var content = getMessages(testReport).JoinToString("\r\n");
        if (string.IsNullOrWhiteSpace(content))
        {
          continue;
        }

        var id = $"{prefix}{counter++}";
        var header = $"" +
                     $"<h4 class='alert-heading'>" +
                     $"  <a href='#{id}'>{id}</a>. <span class='test-name'>{test}</span>" +
                     $"</h4>";

        yield return GetMessage(alertType, header + Render(id, test, content));
      }
    }

    private const string Token = "/--!!--/*--!!--*/--!!--/";

    private static string Render(string id, string testName, string pair)
    {
      var pos = pair.IndexOf(Token);
      if (pos < 0)
      {
        return pair;
      }

      var summary = pair.Substring(0, pos).TrimEnd(" .".ToCharArray());
      var detailed = pair.Substring(pos + Token.Length);

      if (string.IsNullOrWhiteSpace(detailed.Replace("<br />", "").Trim("\r\n ".ToCharArray())))
      {
        return summary;
      }

      return summary + ". " +
        $"  <a href='#' data-toggle='modal' data-target='#{id}-details'>See&nbsp;full&nbsp;details.</a>" +
        $"  <div id='{id}-details' class='modal fade' tabindex='-1' role='dialog' aria-labelledby='myLargeModalLabel' aria-hidden='true'>" +
        $"    <div class='modal-dialog modal-lg'>" +
        $"      <div class='modal-content'>" +
        $"        <div class='modal-header'>" +
        $"          <h5 class='modal-title'>{id}. {testName}</h5>" +
        $"          <button type='button' class='close' data-dismiss='modal' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button>" +
        $"        </div>" +
        $"        <div class='modal-body'>" +
        $"          {summary}" +
        $"          <hr />" +
        $"          {detailed.Replace(Token, "")}" +
        $"        </div>" +
        $"      </div>" +
        $"    </div>" +
        $"  </div>";
    }

    private static string GetValidForText(IEnumerable<KeyValuePair<string, string>> g)
    {
      var text = "";

      foreach (var pair in g)
      {
        var instanceName = pair.Key;
        if (string.IsNullOrEmpty(instanceName))
        {
          continue;
        }

        text += $"<b>Applies to: {instanceName}</b><br />\r\n";
      }

      if (string.IsNullOrWhiteSpace(text))
      {
        return "";
      }

      return $"<hr />{text}";
    }

    private static string GetMessage(string alertType, [NotNull] string contents)
    {
      return $"" +
        $"<div class='alert alert-{alertType} alert-dismissible fade show' role='alert'> " +
        $"  <button type='button' class='close' data-dismiss='alert' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button> " +
        $"  {contents} " +
        $"</div>";
    }
  }
}