namespace Sitecore.DiagnosticsTool
{
  using System;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  using Fclp;

  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.DataProviders.SupportPackage;
  using Sitecore.DiagnosticsTool.Reporting;
  using Sitecore.DiagnosticsTool.TestRunner;

  internal class Program
  {
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
      Console.WriteLine(" 1. Run 'sdt new [-n workplace-name]' to prepare workspace");
      Console.WriteLine("    This command (re)creates .sdt file in current directory");
      Console.WriteLine();
      Console.WriteLine(" 2. For each SSPG package run 'sdt add -p {path-to-sspg} -r {roles-of-given-instance}");
      Console.WriteLine("    For example: ");
      Console.WriteLine("    > sdt add -p C:\\sspg1.zip -r ContentManagement|Processing [-n workplace-name]");
      Console.WriteLine("    > sdt add -p C:\\sspg2.zip -r ContentDelivery [-n workplace-name]");
      Console.WriteLine();
      Console.WriteLine(" 3. Run 'sdt list [-n workplace-name]' to list all added packages and their roles");
      Console.WriteLine();
      Console.WriteLine(" 4. Run 'sdt run -o C:\\output.json [-n workplace-name]' to start analysis");
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

      var filePath = $"{workplaceName}.sdt";
      File.WriteAllText(filePath, "");
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

      var filePath = $"{workplaceName}.sdt";
      var lines = File.ReadAllLines(filePath);
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

      var filePath = $"{workplaceName}.sdt";
      File.AppendAllText(filePath, $"{path}?{string.Join("|", roles.Select(x => x.ToString()))}\r\n");
    }

    private static void RunCommand(string[] options)
    {
      var parser = new FluentCommandLineParser();
      var outputPath = "";
      parser.Setup<string>('o', "output")
        .WithDescription("Output report html file path to be created.")
        .Required()
        .Callback(x => outputPath = x);

      var workplaceName = "";
      parser.Setup<string>('n', "name")
        .WithDescription("Workplace name.")
        .Callback(x => workplaceName = x);

      var result = parser.Parse(options);
      if (result.HelpCalled)
      {
        return;
      }

      var filePath = $"{workplaceName}.sdt";
      if (!File.Exists(filePath))
      {
        ShowHelp();
        return;
      }

      var assemblyName = Assembly.GetExecutingAssembly().GetName();
      var packages = File.ReadAllLines(filePath)
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

      try
      {
        {
          Console.WriteLine("Running tests...");
          var resultsFile = AggregatedTestRunner.RunTests(packages, test => Console.WriteLine($"Running {test?.Name}..."));

          var file = new FileInfo(outputPath);
          file.Directory.Create();

          Console.WriteLine("Building report...");

          File.WriteAllText(
            outputPath,
            ReportBuilder.GenerateReport(resultsFile));
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