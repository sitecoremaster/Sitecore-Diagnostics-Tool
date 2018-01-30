namespace Sitecore.DiagnosticsTool.Commands.Handlers
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;

  using Fclp;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Commands.Contracts;
  using Sitecore.DiagnosticsTool.Core.Extensions;

  internal class CompressCommandHandler : ICompressCommand
  {
    public string[] Options { get; }

    public IFileSystem FileSystem { get; }

    public CompressCommandHandler(ICompressCommand args)
    {
      FileSystem = args.FileSystem ?? Program.FileSystem;
      Options = args.Options;

      Assert.ArgumentNotNull(FileSystem);
      Assert.ArgumentNotNull(Options);
    }

    public void Execute()
    {
      var parser = new FluentCommandLineParser();
      IDirectory mega = null;
      parser.Setup<string>('i', "inputDir")
        .WithDescription("Path to the extracted mega SSPG package folder")
        .Callback(x => mega = FileSystem.ParseDirectory(x));

      var result = parser.Parse(Options);
      if (result.HelpCalled)
      {
        return;
      }

      var onInfo = new Action<string>((msg) => Console.WriteLine($"INFO {msg}"));
      var onError = new Action<string, Exception>((msg, ex) => Console.WriteLine($"ERROR {msg}\r\n{ex.PrintException()}"));

      var folders = mega.GetDirectories();

      var sw = Stopwatch.StartNew();
      ReplaceDuplicateSitecoreKernelsWithLinks(folders, onInfo, onError);

      for (var i = 0; i < folders.Length; i++)
      {
        for (var j = i + 1; j < folders.Length; j++)
        {
          ReplaceDuplicateFilesWithLinks(folders[i], folders[j], onInfo, onError);
        }
      }
      sw.Stop();

      Console.WriteLine($"Done. Elapsed: {sw.Elapsed}");
    }

    private static void ReplaceDuplicateSitecoreKernelsWithLinks(IDirectory[] folders, Action<string> onInfo, Action<string, Exception> onError)
    {
      // replace Version Information/Sitecore.Kernel.dll with link to Assemblies/bin/Sitecore.Kernel.dll if exists
      var binFolder0 = folders[0].GetChildDirectory("Assemblies\\bin");
      ReplaceDuplicateFilesWithLinks(binFolder0, folders[0].GetChildDirectory("Version Information"), onInfo, onError);

      foreach (var anotherFolder in folders.Skip(1))
      {
        // replace Version Information/Sitecore.Kernel.dll with link to Assemblies/bin/Sitecore.Kernel.dll if exists
        ReplaceDuplicateFilesWithLinks(binFolder0, anotherFolder.GetChildDirectory("Version Information"), onInfo, onError);
      }
    }

    private static void ReplaceDuplicateFilesWithLinks([NotNull] IDirectory folder1, [NotNull] IDirectory folder2, [CanBeNull] Action<string> onInfo, [CanBeNull] Action<string, Exception> onError)
    {
      if (!folder1.Exists || !folder2.Exists || string.Equals(Path.GetFullPath(folder1.FullName), Path.GetFullPath(folder2.FullName)))
      {
        return;
      }

      var subfolders1 = folder1.GetDirectories();
      var subfolders2 = folder2.GetDirectories();

      foreach (var subfoldersGroup in subfolders1.Concat(subfolders2).GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
      {
        var subfolders = subfoldersGroup.ToArray();
        if (subfolders.Length == 2)
        {
          ReplaceDuplicateFilesWithLinks(subfolders[0], subfolders[1], onInfo, onError);
        }
      }

      var files1 = folder1.GetFiles();
      var files2 = folder2.GetFiles();

      foreach (var filesGroup in files1.Concat(files2).GroupBy(x => x.Name, StringComparer.OrdinalIgnoreCase))
      {
        var files = filesGroup.ToArray();
        if (files.Length == 2)
        {
          ReplaceDuplicateFilesWithLinks(files[0], files[1], onInfo, onError);
        }
      }
    }

    internal static bool ReplaceDuplicateFilesWithLinks([NotNull] IFile file1, [NotNull] IFile file2, [CanBeNull] Action<string> onInfo, [CanBeNull] Action<string, Exception> onError)
    {
      if (string.Equals(Path.GetFullPath(file1.FullName), Path.GetFullPath(file2.FullName), StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      if (file1.Name.EndsWith(".link", StringComparison.OrdinalIgnoreCase) || file2.Name.EndsWith(".link", StringComparison.OrdinalIgnoreCase))
      {
        return false;
      }

      if (!file1.Exists || !file2.Exists)
      {
        return false;
      }

      try
      {
        if (!FilesContentsAreEqual(file1, file2))
        {
          return false;
        }

        var linkFile = file2.Directory.GetChildFile(file2.Name + ".link");

        try
        {
          onInfo?.Invoke($"Replacing {file2} with link to {file1}");
          var relativePath = GetRelativePath(file1.FullName, file2.FullName);
          var linkFileContents = GetLinkFileContents(relativePath);
          using (var writer = new StreamWriter(linkFile.Open(FileMode.Create, FileAccess.Write, FileShare.Read)))
          {
            writer.Write(linkFileContents);
          }

          try
          {
            file2.Delete();

            return true;
          }
          catch (Exception ex)
          {
            onError?.Invoke($"Failed to delete duplicate file in outer SupportPackage file, duplicate: {file2.FullName}", ex);
          }
        }
        catch (Exception ex)
        {
          onError?.Invoke($"Failed to create a link file in outer SupportPackage file, link: {linkFile.FullName}", ex);

          if (linkFile.Exists)
          {
            linkFile.Delete();
          }
        }
      }
      catch (Exception ex)
      {
        onError?.Invoke($"Failed to replace duplicate file with link in outer SupportPackage file, duplicate: {file2.FullName}", ex);
      }

      return false;
    }

    private static string GetLinkFileContents([NotNull] string relativePath)
    {
      return $"[link] {relativePath}";
    }

    [NotNull]
    internal static string GetRelativePath([NotNull] string path1, [NotNull] string path2)
    {
      var target = Path.GetFullPath(path1);
      var source = Path.GetFullPath(path2);

      if (target.Equals(source, StringComparison.OrdinalIgnoreCase))
      {
        throw new ArgumentException("The path1 and path2 must be different");
      }

      return new Uri(source).MakeRelativeUri(new Uri(target)).ToString();
    }

    private static bool FilesContentsAreEqual([NotNull] IFile file1, [NotNull] IFile file2)
    {
      if (file1.Length != file2.Length)
      {
        return false;
      }

      using (var stream1 = file1.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
      {
        using (var stream2 = file2.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None))
        {
          return StreamsContentsAreEqual(stream1, stream2);
        }
      }
    }

    /// <summary>
    /// Taken from https://stackoverflow.com/questions/968935/compare-binary-files-in-c-sharp#answer-2637303, benchmark gives about 2..10 times faster than answer-968980
    /// </summary>
    private static bool StreamsContentsAreEqual([NotNull] Stream stream1, [NotNull] Stream stream2)
    {
      const int bufferSize = 2048 * 2;
      var buffer1 = new byte[bufferSize];
      var buffer2 = new byte[bufferSize];

      while (true)
      {
        int count1 = stream1.Read(buffer1, 0, bufferSize);
        int count2 = stream2.Read(buffer2, 0, bufferSize);

        if (count1 != count2)
        {
          return false;
        }

        if (count1 == 0)
        {
          return true;
        }

        int iterations = (int)Math.Ceiling((double)count1 / sizeof(Int64));
        for (int i = 0; i < iterations; i++)
        {
          if (BitConverter.ToInt64(buffer1, i * sizeof(Int64)) != BitConverter.ToInt64(buffer2, i * sizeof(Int64)))
          {
            return false;
          }
        }
      }
    }

    private const int RequestLogSize = 100;
    private static readonly List<string> RequestLog = new List<string>();

    internal static bool ShouldProcessRequest([NotNull] string id)
    {
      lock (RequestLog)
      {
        if (RequestLog.Contains(id))
        {
          return false;
        }

        RequestLog.Add(id);

        while (RequestLog.Count > RequestLogSize)
        {
          RequestLog.RemoveAt(0);
        }

        return true;
      }
    }
  }
}