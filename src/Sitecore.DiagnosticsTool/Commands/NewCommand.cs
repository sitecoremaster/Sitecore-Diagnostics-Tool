namespace Sitecore.DiagnosticsTool.Commands
{
  using System;

  using Fclp;

  using JetBrains.Annotations;

  using Sitecore.Diagnostics.Base;
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.Diagnostics.FileSystem.Extensions;

  internal class NewCommandHandler : INewCommand
  {
    [NotNull]
    public IFileSystem FileSystem { get; }

    [NotNull]
    public string[] Options { get; }

    public NewCommandHandler(INewCommand args)
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
      file.WriteAllText("");

      Console.WriteLine("Workspace is created");
    }
  }

  public class NewCommand : INewCommand
  {
    public string[] Options { get; set; }

    public IFileSystem FileSystem { get; set; }

    public void Execute()
    {
      new NewCommandHandler(this).Execute();
    }
  }
}
