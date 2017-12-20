namespace Sitecore.DiagnosticsTool
{
  using System;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Reflection;
  using System.Windows.Forms;

  using Fclp;

  using Sitecore.Diagnostics.Base.Extensions.EnumerableExtensions;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.Reporting;
  using Sitecore.DiagnosticsTool.TestRunner;

  internal class Program
  {
    private static IFileSystem FileSystem { get; } = new FileSystem();

    [STAThread]
    private static void Main(string[] args)
    {
      if (args.Length == 0)
      {
        ShowHelp();

        return;
      }

      var command = args[0];
      var options = args.Skip(1).ToArray();
      Main(command, options);
    }

    private static void ShowHelp()
    {
      Console.WriteLine("Sitecore Diagnostics Tool");
      Console.WriteLine();
      Console.WriteLine("How to use it: ");
      Console.WriteLine(" A. For Sitecore 9.0 mega-package run 'sdt run -p {path-to-sspg} -o C:\\report.html'");
      Console.WriteLine();
      Console.WriteLine(" B. For previous versions, or 9.0 separate pacakges do as follows:");
      Console.WriteLine();
      Console.WriteLine(" B1. Run 'sdt new [-n workplace-name]' to prepare workspace");
      Console.WriteLine("     This command (re)creates .sdt file in current directory");
      Console.WriteLine();
      Console.WriteLine(" B2. For each SSPG package run 'sdt add -p {path-to-sspg} -r {roles-of-given-instance}");
      Console.WriteLine("     For example: ");
      Console.WriteLine("     > sdt add -p C:\\sspg1.zip -r ContentManagement|Processing [-n workplace-name]");
      Console.WriteLine("     > sdt add -p C:\\sspg2.zip -r ContentDelivery [-n workplace-name]");
      Console.WriteLine();
      Console.WriteLine(" B3. Run 'sdt list [-n workplace-name]' to list all added packages and their roles");
      Console.WriteLine();
      Console.WriteLine(" B4. Run 'sdt run -o C:\\report.html [-n workplace-name]' to start analysis");
      Console.WriteLine();
    }

    private static void Main(string command, string[] options)
    {
      switch (command)
      {
        case "new":
          NewCommand(options);
          return;

        case "list":
          ListCommand(options);
          return;

        case "add":
          AddCommand(options);
          return;

        case "run":
          RunCommand(options);
          return;
      }
    }

    private static void NewCommand(string[] options)
    {
      var parser = new FluentCommandLineParser();

      var workplaceName = "";
      parser.Setup<string>('n', "name")
        .WithDescription("Workplace name.")
        .Callback(x => workplaceName = x);

      var result = parser.Parse(options);
      if (result.HelpCalled || result.HasErrors)
      {
        parser.HelpOption.ShowHelp(parser.Options);
        return;
      }

      var file = GetWorkplaceFile(workplaceName);
      file.WriteAllText("");

      Console.WriteLine("Workspace is created");
    }

    private static void ListCommand(string[] options)
    {
      var parser = new FluentCommandLineParser();

      var workplaceName = "";
      parser.Setup<string>('n', "name")
        .WithDescription("Workplace name.")
        .Callback(x => workplaceName = x);

      var result = parser.Parse(options);
      if (result.HelpCalled || result.HasErrors)
      {
        parser.HelpOption.ShowHelp(parser.Options);
        return;
      }

      var file = GetWorkplaceFile(workplaceName);
      var lines = File.ReadAllLines(file.FullName);
      foreach (var line in lines)
      {
        Console.WriteLine(line);
      }
    }

    private static void AddCommand(string[] options)
    {
      var path = "";
      var parser = new FluentCommandLineParser
      {
      };

      parser.SetupHelp("?", "help")
        .Callback(text => Console.WriteLine(text));

      parser.Setup<string>('p', "path")
        .Callback(x => path = x)
        .WithDescription("Path to the SSPG file")
        .Required();

      var roles = new ServerRole[0];
      parser.Setup<string>('r', "role")
        .Callback(x => roles = x.Split(",;|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
          .Select(r => (ServerRole)Enum.Parse(typeof(ServerRole), r))
          .ToArray())
        .WithDescription("One or several pipe-separated server roles: " + string.Join(", ", Enum.GetNames(typeof(ServerRole))))
        .Required();

      var workplaceName = "";
      parser.Setup<string>('n', "name")
        .WithDescription("Workplace name.")
        .Callback(x => workplaceName = x);

      // triggers the SetupHelp Callback which writes the text to the console
      var result = parser.Parse(options);
      if (result.HelpCalled || result.HasErrors)
      {
        parser.HelpOption.ShowHelp(parser.Options);
        return;
      }

      var file = GetWorkplaceFile(workplaceName);
      var rolesText = string.Join("|", roles.Select(x => x.ToString()));
      File.AppendAllText(file.FullName, $"{path}?{rolesText}\r\n");
    }

    private static void RunCommand(string[] options)
    {
      var parser = new FluentCommandLineParser();
      IFile outputFile = null;
      parser.Setup<string>('o', "output")
        .WithDescription("Output report html file path to be created.")
        .Required()
        .Callback(x => outputFile = FileSystem.ParseFile(x));

      var showDialog = false;
      parser.Setup<bool>('d', "diaog")
        .WithDescription("Show popup dialog to choose mega SSPG file")
        .Callback(x => showDialog = x);

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

      var result = parser.Parse(options);
      if (result.HelpCalled)
      {
        return;
      }

      var assemblyName = Assembly.GetExecutingAssembly().GetName();
      IFile workplaceFile = null;
      SupportPackageDataProvider[] packages;
      if (showDialog)
      {
        var dialog = new OpenFileDialog
        {
          Filter = "Mega Support Package|*.zip",
          Multiselect = false,
        };

        var dialogResult = dialog.ShowDialog();

        if (dialogResult == DialogResult.Cancel)
        {
          return;
        }

        mega = FileSystem.ParseFile(dialog.FileName);
      }

      if (mega != null)
      {
        if (!mega.Exists)
        {
          Console.WriteLine($"File does not exist: {mega}");

          return;
        }

        packages = PackageHelper.ExtractMegaPackage(mega)
          .ToArray(x =>
            new SupportPackageDataProvider(x.FullName, null, null, null,
              $"{assemblyName.Name}, {assemblyName.Version.ToString()}"));
      }
      else
      {
        workplaceFile = workplaceFile ?? GetWorkplaceFile(workplaceName);
        if (!workplaceFile.Exists)
        {
          ShowHelp();
          return;
        }

        packages = File.ReadAllLines(workplaceFile.FullName)
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

            return new SupportPackageDataProvider(x.Path, x.Roles, null, null, $"{assemblyName.Name}, {assemblyName.Version.ToString()}");
          })
          .ToArray();
      }

      try
      {
        {
          Console.WriteLine("Running tests...");
          var resultsFile = AggregatedTestRunner.RunTests(packages, (test, index, count) => Console.WriteLine($"Running {test?.Name}..."));
          
          outputFile.Directory.Create();

          Console.WriteLine("Building report...");

          outputFile.WriteAllText(ReportBuilder.GenerateReport(resultsFile));

          if (openReport)
          {
            Process.Start("explorer", $"\"{outputFile}\"");
          }
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

    private static IFile GetWorkplaceFile(string workplaceName)
    {
      return FileSystem.ParseFile($"{workplaceName.TrimEnd(".sdt", StringComparison.OrdinalIgnoreCase)}.sdt");
    }
  }
}