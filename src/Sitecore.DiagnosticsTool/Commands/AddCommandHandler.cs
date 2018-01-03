namespace Sitecore.DiagnosticsTool.Commands
{
  using System;
  using System.Linq;

  using Fclp;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;
  using Sitecore.DiagnosticsTool.Core.Categories;

  internal class AddCommandHandler : IAddCommand
  {
    [NotNull]
    public string[] Options { get; }

    [NotNull]
    public IFileSystem FileSystem { get; }

    public AddCommandHandler(IAddCommand args)
    {
      FileSystem = args.FileSystem ?? Program.FileSystem;
      Options = args.Options;

      Assert.ArgumentNotNull(FileSystem);
      Assert.ArgumentNotNull(Options);
    }

    public void Execute()
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
      var result = parser.Parse(Options);
      if (result.HelpCalled || result.HasErrors)
      {
        parser.HelpOption.ShowHelp(parser.Options);
        return;
      }

      var file = FileSystem.GetWorkplaceFile(workplaceName);
      var rolesText = string.Join("|", roles.Select(x => x.ToString()));
      file.AppendAllText($"{path}?{rolesText}\r\n");
    }
  }
}