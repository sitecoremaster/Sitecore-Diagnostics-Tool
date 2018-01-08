namespace Sitecore.DiagnosticsTool.Commands.Handlers
{
  using System;
  using System.Linq;

  using Fclp;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Commands.Contracts;

  internal class ListCommandHandler : IListCommand
  {
    [NotNull]
    public string[] Options { get; }

    [NotNull]
    public IFileSystem FileSystem { get; }

    public ListCommandHandler(IListCommand args)
    {
      FileSystem = args.FileSystem ?? Program.FileSystem;
      Options = args.Options;

      Assert.ArgumentNotNull(FileSystem);
      Assert.ArgumentNotNull(Options);
    }

    public void Execute()
    {
      var parser = new FluentCommandLineParser();

      var workplaceName = "";
      parser.Setup<string>('n', "name")
        .WithDescription("Workplace name.")
        .Callback(x => workplaceName = x);

      var result = parser.Parse(Options);
      if (result.HelpCalled || result.HasErrors)
      {
        parser.HelpOption.ShowHelp(parser.Options);
        return;
      }

      var file = FileSystem.GetWorkplaceFile(workplaceName);
      if (!file.Exists)
      {
        Console.WriteLine("No workplace created, check 'sdt new' command.");

        return;
      }

      var lines = file.ReadAllLines();
      Console.WriteLine($"Current workplace has {lines.Length} packages added:");
      for (var i = 0; i < lines.Length; i++)
      {
        var line = lines[i];

        var arr = line.Split('?');
        if (arr.Length < 2)
        {
          continue;
        }

        var path = arr[0];
        var roles = string.Join(", ", arr[1].Split(",;|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));

        Console.WriteLine();
        Console.WriteLine($"{i + 1:D1}. Path:  {path}");
        Console.WriteLine($"   Roles: {roles}");
      }
    }
  }
}