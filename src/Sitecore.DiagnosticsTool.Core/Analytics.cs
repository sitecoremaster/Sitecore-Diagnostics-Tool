namespace Sitecore.DiagnosticsTool.Core
{
  using System;
  using System.IO;
  using JetBrains.Annotations;
  using Microsoft.ApplicationInsights;
  using Microsoft.ApplicationInsights.Channel;
  using Microsoft.ApplicationInsights.Extensibility;
  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Logging;

  public static class Analytics
  {
    [CanBeNull]
    private static TelemetryClient telemetryClient;

    public static void Start()
    {
      if (DoNotTrack())
      {
        return;
      }

      Log.Debug("Insights - starting");

      try
      {
        var configuration = TelemetryConfiguration.Active;
        Assert.IsNotNull(configuration, nameof(configuration));

        configuration.TelemetryChannel = new PersistenceChannel("Sitecore Diagnostics Tool");
        configuration.InstrumentationKey = "e55c70b8-4f6b-4efc-b3f6-a75a1054be6a";

        var client = new TelemetryClient(configuration)
        {
          InstrumentationKey = "e55c70b8-4f6b-4efc-b3f6-a75a1054be6a"
        };

        telemetryClient = client;
        try
        {
          // ReSharper disable PossibleNullReferenceException
          client.Context.Component.Version = string.IsNullOrEmpty(Application.AppVersion) ? "0.0.0.0" : Application.AppVersion;
          client.Context.Session.Id = Guid.NewGuid().ToString();
          client.Context.User.Id = Environment.MachineName + "\\" + Environment.UserName;
          client.Context.User.AccountId = GetCookie();
          client.Context.Device.OperatingSystem = Environment.OSVersion.ToString();
          // ReSharper restore PossibleNullReferenceException
          client.TrackEvent("Start");
          client.Flush();
        }
        catch (Exception ex)
        {
          client.TrackException(ex);
          Log.Error(ex, "Error in app insights");
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error in app insights");
      }

      Log.Debug("Insights - started");
    }

    public static void TrackEvent([NotNull] string eventName)
    {
      Assert.ArgumentNotNull(eventName, nameof(eventName));

      var tc = telemetryClient;
      if (tc == null)
      {
        return;
      }

      try
      {
        tc.TrackEvent(eventName);
      }
      catch (Exception ex)
      {
        Log.Error(ex, $"Error during event tracking: {eventName}");
      }
    }

    public static void Flush()
    {
      var tc = telemetryClient;
      if (tc == null)
      {
        return;
      }

      try
      {
        tc.TrackEvent("Exit");

        tc.Flush();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Error during flushing");
      }
    }

    public static bool DoNotTrack()
    {
      var path = Path.Combine(Application.TempFolder, "donottrack.txt");

      return File.Exists(path);
    }

    [NotNull]
    public static string GetCookie()
    {
      var tempFolder = Application.TempFolder;
      var path = Path.Combine(tempFolder, "cookie.txt");
      if (Directory.Exists(tempFolder))
      {
        if (File.Exists(path))
        {
          var cookie = File.ReadAllText(path);
          if (!string.IsNullOrEmpty(cookie))
          {
            return cookie;
          }

          try
          {
            File.Delete(path);
          }
          catch (Exception ex)
          {
            Log.Error(ex, "Cannot delete cookie file");
          }
        }
      }
      else
      {
        Directory.CreateDirectory(tempFolder);
      }

      var newCookie = Guid.NewGuid().ToString("D");
      try
      {
        File.WriteAllText(path, newCookie);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "Cannot write cookie");
      }

      return newCookie;
    }
  }
}