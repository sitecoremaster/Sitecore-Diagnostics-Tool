namespace Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources.Logging
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Xml;
  using JetBrains.Annotations;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.LogAnalyzer;
  using Sitecore.Diagnostics.LogAnalyzer.Managers;
  using Sitecore.Diagnostics.LogAnalyzer.Models;
  using Sitecore.Diagnostics.LogAnalyzer.Parsing;
  using Sitecore.Diagnostics.LogAnalyzer.Settings;
  using Sitecore.Diagnostics.LogAnalyzer.States;
  using Sitecore.Diagnostics.Logging;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.Core.Resources.Common;
  using Sitecore.DiagnosticsTool.Core.Resources.Logging;

  public class LoggingContext : ILoggingContext
  {
    private const long SitecoreLogsLimit = 209715200;

    [NotNull]
    private readonly LogProcessorEx _LogProcessor = new LogProcessorEx();

    private IReadOnlyList<ILogEntry> _SitecoreLogs;

    protected LoggingContext([NotNull] string sitecoreLogsFolderPath)
    {
      Assert.ArgumentNotNull(sitecoreLogsFolderPath, nameof(sitecoreLogsFolderPath));

      Configuration.Initialize();
      SitecoreLogsFolderPath = sitecoreLogsFolderPath;
    }

    [NotNull]
    public string SitecoreLogsFolderPath { get; }

    public IEnumerable<ILogEntry> GetSitecoreLogEntries()
    {
      return GetSitecoreLogEntries(Core.Resources.Logging.LogLevel.All, DateTime.MinValue, DateTime.MaxValue);
    }

    public IEnumerable<ILogEntry> GetSitecoreLogEntries(Core.Resources.Logging.LogLevel logLevel)
    {
      return GetSitecoreLogEntries(logLevel, DateTime.MinValue, DateTime.MaxValue);
    }

    public IEnumerable<ILogEntry> GetSitecoreLogEntries(Core.Resources.Logging.LogLevel logLevel, DateTime start, DateTime end)
    {
      var cache = _SitecoreLogs;
      if (cache == null)
      {
        var logProcessorSettings = new LogProcessorSettings
        {
          ConnectionSettings = new FileConnectionSettings(GetLogFilesByLimit()),
          ReaderSettings = new LogReaderSettings(string.Empty)
        };

        var logs = _LogProcessor.Analyze(logProcessorSettings);
        if (logs == null || logs.All == null)
        {
          throw new LogFilesResourceNotAvailableException("Sitecore logs were not found, corrupted or their limit (200MB) has been exceeded during loading");
        }

        cache = logs.All.Select(x => new LogEntryEx(x.IsNotNull("x"))).ToArray();
        _SitecoreLogs = cache;
      }

      return logLevel.HasFlag(Core.Resources.Logging.LogLevel.Debug) ? cache.Where(x => start <= x.Date && x.Date <= end) : cache.Where(x => x.Level.HasFlag(logLevel) && start <= x.Date && x.Date <= end);
    }

    private IEnumerable<FileInfo> GetAllLogFiles()
    {
      return new DirectoryInfo(SitecoreLogsFolderPath).GetFiles().OrderBy(x => x.CreationTime);
    }

    private FileInfo[] GetLogFilesByLimit()
    {
      if (GetAllLogFiles().Sum(log => log.Length) <= SitecoreLogsLimit)
      {
        return GetAllLogFiles().ToArray();
      }

      var totalSize = 0L;
      var validLogFiles = new List<FileInfo>();

      var logTypes = new List<FileInfo[]>
      {
        GetLogFilesByType(LogType.Log).Reverse().ToArray(),
        GetLogFilesByType(LogType.Publishing).Reverse().ToArray(),
        GetLogFilesByType(LogType.Crawling).Reverse().ToArray(),
        GetLogFilesByType(LogType.Search).Reverse().ToArray(),
        GetLogFilesByType(LogType.Fxm).Reverse().ToArray(),
        GetLogFilesByType(LogType.Client).Reverse().ToArray(),
        GetLogFilesByType(LogType.WebDav).Reverse().ToArray(),
        GetLogFilesByType(LogType.Custom).Reverse().ToArray()
      };

      var iterrator = 0;

      while (totalSize < SitecoreLogsLimit)
      {
        foreach (var logType in logTypes)
        {
          if (iterrator >= logType.Length)
          {
            continue;
          }
          var logItem = logType[iterrator];

          if (totalSize + logItem.Length < SitecoreLogsLimit)
          {
            validLogFiles.Add(logItem);
            totalSize += logItem.Length;
            Log.Info($"Loading log file: {logItem.Name} (file: {logItem.Length / 1048576}MB, total: {totalSize / 1048576}MB)");
          }
          else
          {
            Log.Warn($"Sitecore logs limit (200MB) has been exceeded during loading the file: {logItem.Name} (file: {logItem.Length / 1048576}MB, total: {(totalSize + logItem.Length) / 1048576}MB)");
            goto ReturnLogs;
          }
        }

        iterrator++;
      }

      ReturnLogs:
      return validLogFiles.ToArray();
    }

    private IEnumerable<FileInfo> GetLogFilesByType(LogType type)
    {
      var all = GetAllLogFiles();

      switch (type)
      {
        case LogType.Crawling:
          return all.Where(l => l.Name.StartsWith("Crawling", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.Log:
          return all.Where(l => l.Name.StartsWith("Log", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.Publishing:
          return all.Where(l => l.Name.StartsWith("Publishing", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.Search:
          return all.Where(l => l.Name.StartsWith("Search", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.WebDav:
          return all.Where(l => l.Name.StartsWith("WebDAV", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.Fxm:
          return all.Where(l => l.Name.StartsWith("Fxm", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.Client:
          return all.Where(l => l.Name.StartsWith("Client", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        case LogType.Custom:
          return all.Where(l => !l.Name.StartsWith("Crawling", StringComparison.CurrentCultureIgnoreCase) &&
            !l.Name.StartsWith("Log", StringComparison.CurrentCultureIgnoreCase) &&
            !l.Name.StartsWith("Publishing", StringComparison.CurrentCultureIgnoreCase) &&
            !l.Name.StartsWith("Search", StringComparison.CurrentCultureIgnoreCase) &&
            !l.Name.StartsWith("WebDav", StringComparison.CurrentCultureIgnoreCase)).ToArray();
        default:
          return null;
      }
    }

    private static IEnumerable<LogEntry> FilterLogs([NotNull] ParsingResult logs, Core.Resources.Logging.LogLevel logLevel)
    {
      Assert.ArgumentNotNull(logs, nameof(logs));
      List<LogEntry> filteredLogs = null;
      switch (logLevel)
      {
        case Core.Resources.Logging.LogLevel.Fatal:
          filteredLogs = logs.Fatals;
          break;

        case Core.Resources.Logging.LogLevel.Error:
          filteredLogs = logs.Errors;
          break;

        case Core.Resources.Logging.LogLevel.Warn:
          filteredLogs = logs.Warns;
          break;

        case Core.Resources.Logging.LogLevel.Info:
          filteredLogs = logs.Infos;
          break;

        case Core.Resources.Logging.LogLevel.Debug:
          filteredLogs = logs.Debugs;
          break;

        case Core.Resources.Logging.LogLevel.All:
          filteredLogs = logs.All;
          break;
      }

      return filteredLogs;
    }

    public static LoggingContext ParseFolder([CanBeNull] string logFolderPath)
    {
      if (logFolderPath == null || !Directory.Exists(logFolderPath))
      {
        Log.Info($"The log folder path setting points to {logFolderPath}");

        return null;
      }

      var context = new LoggingContext(logFolderPath);

      return context;
    }

    public static LoggingContext Parse(XmlDocument configuration, string webRootPath, string dataFolderPath)
    {
      var xpath = "/configuration/sitecore/settings/setting[@name='LogFolder']";
      var setting = configuration.SelectSingleNode(xpath);
      if (setting == null)
      {
        Log.Warn($"The setting is missing in the configuration: {xpath}");

        var defaultLogFolderPath = Path.Combine(dataFolderPath, "logs");

        return ParseFolder(defaultLogFolderPath);
      }

      var settingValue = setting
        .With(x => x.Attributes)
        .With(x => x["value"])
        .With(x => x.Value);

      if (string.IsNullOrEmpty(settingValue))
      {
        Log.Warn("The <setting name='LogFolder' /> setting value attribute is missing or contains empty value");

        var defaultLogFolderPath = Path.Combine(dataFolderPath, "logs");

        return ParseFolder(defaultLogFolderPath);
      }

      var logFolderPath = settingValue;
      if (logFolderPath.StartsWith("~\\", StringComparison.InvariantCultureIgnoreCase) || logFolderPath.StartsWith("~/", StringComparison.CurrentCultureIgnoreCase))
      {
        logFolderPath = logFolderPath.TrimStart('~');
      }

      logFolderPath = logFolderPath.TrimStart(Path.DirectorySeparatorChar);
      logFolderPath = logFolderPath.TrimStart(Path.AltDirectorySeparatorChar);

      if (string.IsNullOrEmpty(logFolderPath))
      {
        Log.Error($"Log folder path points to the website root folder: {settingValue}");

        return null;
      }

      if (!Path.IsPathRooted(logFolderPath))
      {
        logFolderPath = Path.Combine(webRootPath, logFolderPath);
      }

      return ParseFolder(logFolderPath);
    }

    /// <summary>
    ///   Wrapper that gets rid of SCLA IoC.
    /// </summary>
    private class LogProcessorEx : LogProcessor
    {
      [NotNull]
      private static readonly ICaptionManager CaptionManager = new CaptionManager();

      // must reside after FileHelper and DiagnosticsHelper
      [NotNull]
      private static readonly LogSourceFactory LogSourceFactory = new LogSourceFactory();

      // must reside after LogParser, AuditParser, ErrorParser and HealthMonitorParser
      [NotNull]
      private static readonly LogReader LogReader = new LogReader();

      // must reside after CaptionManager
      [NotNull]
      private new static readonly ContextFactory ContextFactory = new ContextFactory(CaptionManager);

      public LogProcessorEx()
        : base(LogSourceFactory, LogReader, ContextFactory, CaptionManager)
      {
      }

      public new ParsingResult Analyze([NotNull] LogProcessorSettings settings)
      {
        Assert.ArgumentNotNull(settings, nameof(settings));
        var context = new ProcessContext
        {
          Settings = settings
        };
        return ParseLogsWrap(context, context.Logging);
      }
    }
  }
}