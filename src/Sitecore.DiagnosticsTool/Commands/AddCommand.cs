namespace Sitecore.DiagnosticsTool.Commands
{
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.DiagnosticsTool.Commands.Contracts;
  using Sitecore.DiagnosticsTool.Commands.Handlers;

  internal class AddCommand : IAddCommand
  {
    public string[] Options { get; set; }

    public IFileSystem FileSystem { get; set; }

    public void Execute()
    {
      new AddCommandHandler(this).Execute();
    }
  }
}