namespace Sitecore.DiagnosticsTool.Core
{
  using System;
  using System.IO;
  using System.Reflection;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;

  public static class Application
  {
    private static string appName;

    private static string appVersion;

    static Application()
    {
      if (!Directory.Exists(TempFolder))
      {
        Directory.CreateDirectory(TempFolder);
      }
    }

    [NotNull]
    public static readonly string TempFolder = Environment.ExpandEnvironmentVariables(@"%APPDATA%\Sitecore\Sitecore Diagnostics Tool\Temp");

    [NotNull]
    [PublicAPI]
    public static string AppName
    {
      get
      {
        var name = appName;
        if (string.IsNullOrEmpty(name))
        {
          throw new InvalidOperationException("Application is not initialized");
        }

        return name;
      }
    }

    [NotNull]
    [PublicAPI]
    public static string AppVersion
    {
      get
      {
        var version = appVersion;
        if (string.IsNullOrEmpty(version))
        {
          throw new InvalidOperationException("Application is not initialized");
        }

        return version;
      }
    }

    [PublicAPI]
    public static event EventHandler OnExit;

    public static void Initialize([NotNull] string name, [NotNull] string version)
    {
      Assert.ArgumentNotNull(name, nameof(name));
      Assert.ArgumentNotNull(version, nameof(version));

      appName = name;
      appVersion = version;
    }

    public static void Exit()
    {
      OnExit?.Invoke(null, null);
    }

    [NotNull]
    public static string GetEmbeddedFile([NotNull] Assembly assembly, [NotNull] string fileName)
    {
      Assert.ArgumentNotNull(assembly, nameof(assembly));
      Assert.ArgumentNotNull(fileName, nameof(fileName));

      var assemblyName = assembly.GetName().Name;
      Assert.IsNotNullOrEmpty(assemblyName, nameof(assemblyName));

      var folder = Path.Combine(TempFolder, assemblyName);
      if (!Directory.Exists(folder))
      {
        Directory.CreateDirectory(folder);
      }

      var filePath = Path.Combine(folder, fileName);
      if (File.Exists(filePath))
      {
        File.Delete(filePath);
      }

      using (var stream = assembly.GetManifestResourceStream(assemblyName + @"." + fileName))
      {
        Assert.IsNotNull(stream, nameof(stream));

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
          const int BufferSize = 2048;

          int len;
          var buffer = new byte[BufferSize];

          while ((len = stream.Read(buffer, 0, BufferSize)) > 0)
          {
            fileStream.Write(buffer, 0, len);
          }
        }

        Assert.IsTrue(File.Exists(filePath), $"The {filePath} file path doesn't exist after successful extracting {filePath} package into {folder} folder");

        return filePath;
      }
    }
  }
}