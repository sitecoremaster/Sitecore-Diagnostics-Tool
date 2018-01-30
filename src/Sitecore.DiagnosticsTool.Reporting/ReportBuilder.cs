namespace Sitecore.DiagnosticsTool.Reporting
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;

  using JetBrains.Annotations;

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

      var errorMessages = Safe(_ => GetErrorMessages(resultsFile)) ?? new string[0];
      var errorText = GetMessagesText(errorMessages, "error");

      var warningMessages = Safe(_ => GetWarningMessages(resultsFile)) ?? new string[0];
      var warningText = GetMessagesText(warningMessages, "warning");

      var debugMessages = Safe(_ => GetDebugMessages(resultsFile)) ?? new string[0];
      var debugText = GetMessagesText(debugMessages, "debug");

      var cannotRunMessages = Safe(_ => GetCannotRunMessages(resultsFile)) ?? new string[0];
      var cannotRunText = GetMessagesText(cannotRunMessages, "cannot run");

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

    internal static string GetMessagesText(string[] errorMessages, string name)
    {
      return errorMessages
        .JoinToString("\r\n").EmptyToNull() ?? $"<p>No {name.ToLower()} messages</p>";
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
      var count = errorMessages + warningMessages + resultsFile.Solution.Count(z => z.Results.DebugLogs.Any(c => c != null));

      return $"{count}";
    }

    private static string GetRunTestsCount(ResultsFile resultsFile)
    {
      var runTestsCount = resultsFile.Solution
        .Where(x => !x.Results.CannotRun.Any())
        .GroupBy(x => x.Owner.Name)
        .Count();

      return $"{runTestsCount}";
    }

    private static string GetTotalTestsCount()
    {
      return new TestManager().GetTests().Count().ToString();
    }

    private static string GetInstancesCount(ResultsFile resultsFile)
    {
      var instancesCount = $"{resultsFile.Packages?.Count ?? -1}";

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
    internal static string[] GetErrorMessages(ResultsFile resultsFile)
    {
      return GetMessages("danger", "E", resultsFile, r => r.Results.Errors).ToArray();
    }

    [NotNull]
    private static string[] GetWarningMessages(ResultsFile resultsFile)
    {
      return GetMessages("warning", "W", resultsFile, r => r.Results.Warnings).ToArray();
    }

    [NotNull]
    private static string[] GetDebugMessages(ResultsFile resultsFile)
    {
      return GetMessages("info", "D", resultsFile, r => r.Results.DebugLogs.Select(GetDebugMessages)).ToArray();
    }

    private static ITestResultData GetDebugMessages(Container m)
    {
      if (m.Items.Length == 1 && m.Items.Single() is Text)
      {
        return new TestOutputData
        {
          Message = new ShortMessage(m.Items.Single()),
          Detailed = new DetailedMessage(m.Items.Single()),
        };
      }

      return new TestOutputData
      {
        Message = "There is an additional debugging information",
        Detailed = new DetailedMessage(m)
      };
    }

    [NotNull]
    private static string[] GetCannotRunMessages(ResultsFile resultsFile)
    {
      return GetMessages("info", "C", resultsFile, r => r.Results.CannotRun).ToArray();
    }

    [NotNull]
    [ItemNotNull]
    private static IEnumerable<string> GetMessages(string alertType, string prefix, ResultsFile resultsFile, Func<ITestReport, IEnumerable<ITestResultData>> getMessages)
    {
      var counter = 1;
      foreach (var group in resultsFile.Solution.GroupBy(x => x.Owner.Name))
      {
        var test = group.Key;

        // e.g. all error messages from this test
        var messages = group.SelectMany(getMessages);

        // data grouped by short messages, and every group has dictionary [ instance => detailed messages[] ]
        var data = messages
          .GroupBy(x => x.Message.ToString(OutputFormat.Html))
          .ToArray(g => new
          {
            Message = g.Key,
            PerInstance = g
              .GroupBy(x => x.Instance)
              .ToDictionary(x => x.Key ?? "", x => x.ToArray())
          });

        if (!data.Any())
        {
          continue;
        }

        var shortMessages = new List<string>();
        var detailedMessages = new List<string>();

        foreach (var messageGroup in data)
        {
          var shortMessage = messageGroup.Message;
          if (string.IsNullOrWhiteSpace(shortMessage.Replace("  ", " ").Replace("<br />", "").Replace("<br>", "")))
          {
            continue;
          }

          shortMessages.Add(shortMessage);

          foreach (var instanceGroup in messageGroup.PerInstance)
          {
            var instanceName = instanceGroup.Key;

            var detailedMessage = "";
            if (!string.IsNullOrEmpty(instanceName))
            {
              detailedMessage += $"<div class='applies-to'>{instanceName}</div>";
            }

            var detailedMessages1 = instanceGroup.Value
              .Select(x => RenderDetailedMessage(x, shortMessage))
              .GroupBy(x => x) // to remove duplicates
              .ToArray(x => x.Key);

            detailedMessage += detailedMessages1.JoinToString("");

            detailedMessages.Add(detailedMessage);
          }
        }

        var id = $"{prefix}{counter++}";

        var header = $"<h4 class='alert-heading'>" +
          $"  <a href='#{id}'>{id}</a>. <span class='test-name'>{test}</span>" +
          $"</h4>";

        var briefView = $"<div class=\'short-message\'>{string.Join("</div><hr /><div class='short-message'>", shortMessages)}</div>";

        if (detailedMessages.All(string.IsNullOrWhiteSpace))
        {
          yield return GetMessage(alertType, header + briefView);

          continue;
        }

        if (AreSame(shortMessages, detailedMessages))
        {
          yield return GetMessage(alertType, header + briefView);

          continue;
        }

        var detailedView = $"<div class=\'detailed-message\'>{string.Join("</div><hr /><div class='detailed-message'>", detailedMessages)}</div>";
        var html = $"" +
          $"{header}" +
          $"{briefView}" +
          $"<a href='#' data-toggle='modal' data-target='#{id}-details'>See&nbsp;full&nbsp;details.</a>" +
          $"  <div id='{id}-details' class='modal fade' tabindex='-1' role='dialog' aria-labelledby='myLargeModalLabel' aria-hidden='true'>" +
          $"    <div class='modal-dialog modal-lg'>" +
          $"      <div class='modal-content'>" +
          $"        <div class='modal-header'>" +
          $"          <h5 class='modal-title'>{id}. {test}</h5>" +
          $"          <button type='button' class='close' data-dismiss='modal' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button>" +
          $"        </div>" +
          $"        <div class='modal-body'>" +
          $"          {detailedView}" +
          $"        </div>" +
          $"      </div>" +
          $"    </div>" +
          $"  </div>";

        yield return GetMessage(alertType, html);
      }
    }

    private static bool AreSame(List<string> first, List<string> second)
    {
      if (first.Count != second.Count)
      {
        return false;
      }

      for (var i = 0; i < first.Count; ++i)
      {
        if (first[i] != second[i])
        {
          return false;
        }
      }

      return true;
    }

    private static string RenderDetailedMessage(ITestResultData testResult, string shortMessage)
    {
      var result = "";
      var detailed = testResult.Detailed;
      var uri = testResult.Link;
      if (detailed != null)
      {
        result += detailed.ToString(OutputFormat.Html);
      }
      else
      {
        result += shortMessage;
      }

      if (uri != null)
      {
        result += $"" +
          $"<div>" +
          $"Get more information in <a href='{uri.AbsoluteUri}'>this document</a>." +
          $"</div>";
      }

      return result;
    }

    private static string GetMessage(string alertType, [NotNull] string contents)
    {
      return $"" +
        $"<div class='alert alert-{alertType} alert-dismissible fade show' role='alert'> " +
        $"  <button type='button' class='close' data-dismiss='alert' aria-label='Close'> <span aria-hidden='true'>&times;</span> </button> " +
        $"  {contents} " +
        $"</div>";
    }

    public class TestOutputData : ITestResultData
    {
      public ShortMessage Message { get; set; }

      public Uri Link { get; set; }

      public DetailedMessage Detailed { get; set; }

      public string Instance { get; set; }
    }
  }
}