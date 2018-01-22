namespace Sitecore.DiagnosticsTool
{
  using System;
  using System.Linq;

  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.DiagnosticsTool.Commands;
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

      switch (command)
      {
        case "new":
          new NewCommand { Options = options }.Execute();

          return;

        case "list":
          new ListCommand { Options = options }.Execute();

          return;

        case "add":
          new AddCommand { Options = options }.Execute();

          return;

        case "compress":
          new CompressCommand { Options = options }.Execute();

          return;

        case "decompress":
          new DecompressCommand { Options = options }.Execute();

          return;

        case "run":
          new RunCommand { Options = options }.Execute();

          return;

        default:
          throw new NotSupportedException();
      }
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

    internal static IFile GetWorkplaceFile(this IFileSystem fileSystem, string workplaceName)
    {
      if (string.IsNullOrWhiteSpace(workplaceName))
      {
        workplaceName = "";
      }
      
      return fileSystem.ParseFile($"{workplaceName.TrimEnd(".sdt", StringComparison.OrdinalIgnoreCase)}.sdt");
    }
  }
}