namespace Sitecore.DiagnosticsTool
{
  using System;
  using System.IO;
  using System.Linq;

  using Fclp;

  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Commands;
  using Sitecore.DiagnosticsTool.Core.Categories;
  using Sitecore.DiagnosticsTool.Core.Extensions;

  internal static class Program
  {
    internal static IFileSystem FileSystem { get; } = new FileSystem();

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

    internal static void ShowHelp()
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
          new NewCommand { Options = options }.Execute();
          return;

        case "list":
          ListCommand(options);
          return;

        case "add":
          AddCommand(options);
          return;

        case "run":
          new RunCommand { Options = options }.Execute();

          return;
      }
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

      var file = FileSystem.GetWorkplaceFile(workplaceName);
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

      var file = FileSystem.GetWorkplaceFile(workplaceName);
      var rolesText = string.Join("|", roles.Select(x => x.ToString()));
      File.AppendAllText(file.FullName, $"{path}?{rolesText}\r\n");
    }
    
    internal static IFile GetWorkplaceFile(this IFileSystem fileSystem, string workplaceName)
    {
      return fileSystem.ParseFile($"{workplaceName.TrimEnd(".sdt", StringComparison.OrdinalIgnoreCase)}.sdt");
    }
  }
}