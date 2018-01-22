namespace Sitecore.DiagnosticsTool.Commands
{
  using Sitecore.Diagnostics.FileSystem;
  using Sitecore.DiagnosticsTool.Commands.Contracts;
  using Sitecore.DiagnosticsTool.Commands.Handlers;

  public class DecompressCommand : ICompressCommand
  {
    public string[] Options { get; set; }

    public IFileSystem FileSystem { get; set; }

    public void Execute()
    {
      new DecompressCommandHandler(this).Execute();
    }
  }
}