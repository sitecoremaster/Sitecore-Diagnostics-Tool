namespace Sitecore.DiagnosticsTool.Commands.Handlers
{
  using System;
  using System.Diagnostics;
  using System.Linq;
  using System.Reflection;
  using System.Windows.Forms;

  using Fclp;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Commands.Contracts;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage.Resources;
  using Sitecore.DiagnosticsTool.Reporting;

  internal class RunCommandHandler : IRunCommand
  {
    [NotNull]
    public string[] Options { get; }

    [NotNull]
    public IFileSystem FileSystem { get; }

    public RunCommandHandler(IRunCommand args)
    {
      FileSystem = args.FileSystem ?? Program.FileSystem;
      Options = args.Options;

      Assert.ArgumentNotNull(FileSystem);
      Assert.ArgumentNotNull(Options);
    }

    public void Execute()
    {
      var parser = new FluentCommandLineParser();
      IFile outputFile = null;
      parser.Setup<string>('o', "output")
        .WithDescription("Output report html file path to be created.")
        .Required()
        .Callback(x => outputFile = FileSystem.ParseFile(x));

      var fileDialog = false;
      parser.Setup<bool>('f', "fileDialog")
        .WithDescription("Show popup dialog to choose mega SSPG file")
        .Callback(x => fileDialog = x);

      var dirDialog = false;
      parser.Setup<bool>('d', "directoryDialog")
        .WithDescription("Show popup dialog to choose extracted mega SSPG file")
        .Callback(x => dirDialog = x);

      var openReport = false;
      parser.Setup<bool>('e', "open")
        .WithDescription("Open report after generating")
        .Callback(x => openReport = x);

      IFile mega = null;
      parser.Setup<string>('p', "package")
        .WithDescription("Path to the mega SSPG package file")
        .Callback(x => mega = FileSystem.ParseFile(x));

      var workplaceName = "";
      parser.Setup<string>('n', "name")
        .WithDescription("Workplace name.")
        .Callback(x => workplaceName = x);

      var result = parser.Parse(Options);
      if (result.HelpCalled)
      {
        return;
      }

      var assemblyName = Assembly.GetExecutingAssembly().GetName().ToString();
      var system = new SystemContext(assemblyName);
      IFile workplaceFile = null;
      SupportPackageDataProvider[] packages = null;
      if (fileDialog)
      {
        var dialog = new OpenFileDialog
        {
          Filter = "Mega Support Package|*.zip|Diagnostics Tool Workspace|*.sdt",
          Multiselect = false,
        };

        IFile file;
        while (true)
        {
          var dialogResult = dialog.ShowDialog();

          if (dialogResult == DialogResult.Cancel)
          {
            return;
          }

          file = FileSystem.ParseFile(dialog.FileName);
          if (file.Exists)
          {
            break;
          }
        }

        if (file.Extension.Equals(".sdt", StringComparison.OrdinalIgnoreCase))
        {
          mega = null;
          workplaceFile = file;
        }
        else
        {
          mega = file;
        }
      }
      else if (dirDialog)
      {
        var dialog = new FolderBrowserDialog();
        var lastPathFile = FileSystem.ParseFile(Environment.ExpandEnvironmentVariables(@"%APPDATA%\Sitecore\Sitecore Diagnostics Tool\Temp\directoryDialog-LastPath.txt"));
        if (lastPathFile.Exists)
        {
          var value = lastPathFile.ReadAllText();
          while (!string.IsNullOrEmpty(value))
          {
            if (FileSystem.ParseDirectory(value).Exists)
            {
              dialog.SelectedPath = value;
              break;
            }

            value = string.Join("\\", value.Split('\\').Reverse().Skip(1).Reverse());
          }
        }

        IDirectory dir;
        while (true)
        {
          var dialogResult = dialog.ShowDialog();

          if (dialogResult == DialogResult.Cancel)
          {
            return;
          }

          dir = FileSystem.ParseDirectory(dialog.SelectedPath);
          if (dir.Exists)
          {
            break;
          }
        }

        packages = dir.GetDirectories()
          .ToArray(x =>
            new SupportPackageDataProvider(x, null, null));
      }

      if (packages == null)
      {
        if (mega != null)
        {
          if (!mega.Exists)
          {
            Console.WriteLine($"File does not exist: {mega}");

            return;
          }

          packages = PackageHelper.ExtractMegaPackage(mega)
            .ToArray(x =>
              new SupportPackageDataProvider(x, null, null));
        }
      }

      if (packages == null)
      {
        workplaceFile = workplaceFile ?? FileSystem.GetWorkplaceFile(workplaceName);
        if (!workplaceFile.Exists)
        {
          Program.ShowHelp();
          return;
        }

        packages = workplaceFile.ReadAllLines()
          .Select(x => x.Split('?'))
          .Select(x => new
          {
            Path = x[0],
            Roles = x[1].Split('|')
              .Select(r => (ServerRole)Enum.Parse(typeof(ServerRole), r))
              .ToArray()
          })
          .Select(x =>
          {
            Console.WriteLine($"Parsing {x.Path}");

            var file = FileSystem.ParseFile(x.Path);
            var dir = FileSystem.ParseDirectory(x.Path);
            Assert.IsTrue(file.Exists || dir.Exists, "Neither file nor dir exists: " + x.Path);

            var fileSystemEntry = file.Exists ? (IFileSystemEntry)file : dir;

            return new SupportPackageDataProvider(fileSystemEntry, x.Roles, null);
          })
          .ToArray();
      }

      try
      {
        Console.WriteLine("Running tests...");
        var resultsFile = TestRunner.TestRunner.RunTests(packages, system, (test, index, count) => Console.WriteLine($"Running {test?.Name}..."));

        outputFile.Directory.Create();

        Console.WriteLine("Building report...");

        outputFile.WriteAllText(ReportBuilder.GenerateReport(resultsFile));

        if (openReport)
        {
          Process.Start("explorer", $"\"{outputFile}\"");
        }
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
